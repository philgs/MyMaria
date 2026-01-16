using System;
using System.Management.Automation;
using MySql.Data.MySqlClient;

namespace MyMaria
{
    [Cmdlet(VerbsCommunications.Disconnect, "Database")]
    [Alias("dcmydb")]
    public class DisconnectDatabaseCommand : PSCmdlet
    {
        [Parameter(Position = 0, ValueFromPipeline = true)]
        public MySqlConnection Connection { get; set; } = null;

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
                    new InvalidOperationException("No connection to disconnect."),
                    "NoConnection",
                    ErrorCategory.InvalidOperation,
                    null
                ));
                return;
            }

            try
            {
                if (Connection.State != System.Data.ConnectionState.Closed)
                    Connection.Close();

                Connection.Dispose();
                WriteVerbose("Database connection closed and disposed.");
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "DatabaseDisconnectionFailed", ErrorCategory.CloseError, Connection));
            }
            finally
            {
                SessionState.PSVariable.Remove(PSModuleConstants.ConnectionVariable);
            }
        }
    }
}
