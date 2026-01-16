using System;
using System.Management.Automation;
using MySql.Data.MySqlClient;

namespace MyMaria
{
    [Cmdlet(VerbsLifecycle.Invoke, "Query")]
    [Alias("imyqry")]
    public class InvokeQueryCommand : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public string Query { get; set; }

        [Parameter(Mandatory = false)]
        public MySqlConnection Connection { get; set; } = null;

        [Parameter(Mandatory = false)]
        [ValidateRange(0, int.MaxValue)]
        public int CommandTimeout { get; set; } = 30;

        protected override void BeginProcessing()
        {
            if (Connection == null)
                Connection = (MySqlConnection)SessionState.PSVariable.GetValue(PSModuleConstants.ConnectionVariable);
        }

        protected override void ProcessRecord()
        {
            if (Connection == null)
            {
                WriteError(new ErrorRecord(
                    new InvalidOperationException("No connection available. Use Connect-Database first or provide a Connection."),
                    "NoConnection",
                    ErrorCategory.ConnectionError,
                    null
                ));
                return;
            }

            if (Connection.State != System.Data.ConnectionState.Open)
            {
                WriteError(new ErrorRecord(
                    new InvalidOperationException($"Connection is not open. Current state: {Connection.State}"),
                    "ConnectionNotOpen",
                    ErrorCategory.ConnectionError,
                    Connection
                ));
                return;
            }

            try
            {
                using (var command = new MySqlCommand(Query, Connection) { CommandTimeout = CommandTimeout })
                using (var reader = command.ExecuteReader())
                {
                    int rowCount = 0;
                    int fieldCount = reader.FieldCount;
                    while (reader.Read())
                    {
                        var psObj = new PSObject();
                        for (int i = 0; i < fieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            psObj.Properties.Add(new PSNoteProperty(columnName, value));
                        }

                        WriteObject(psObj);
                        rowCount++;
                    }

                    WriteVerbose($"Query returned {rowCount} record(s).");
                }
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "QueryExecutionFailed", ErrorCategory.ReadError, null));
            }
        }
    }
}
