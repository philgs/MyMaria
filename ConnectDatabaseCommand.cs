using System;
using System.Management.Automation;
using MySql.Data.MySqlClient;

namespace MyMaria
{
    [CmdletBinding]
    [Cmdlet(VerbsCommunications.Connect, "Database")]
    [OutputType(typeof(MySqlConnection))]
    [Alias("cnmydb")]
    public class ConnectDatabaseCommand : PSCmdlet
    {
        [Parameter(Position = 0)]
        [ValidateNotNullOrEmpty]
        [Alias("Server")]
        public string ComputerName { get; set; } = "localhost";

        [Parameter(Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string Database { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public PSCredential Credential { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(1, 65535)]
        public uint Port { get; set; } = 3306;

        [Parameter(Mandatory = false)]
        [ValidateRange(0, int.MaxValue)]
        public uint CommandTimeout { get; set; } = 60;

        [Parameter(Mandatory = false)]
        [ValidateRange(0, int.MaxValue)]
        public uint ConnectionTimeout { get; set; } = 30;

        protected override void ProcessRecord()
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder
                {
                    Server = ComputerName,
                    Port = Port,
                    UserID = Credential.UserName,
                    Password = Credential.GetNetworkCredential().Password,
                    DefaultCommandTimeout = CommandTimeout,
                    ConnectionTimeout = ConnectionTimeout,
                    AllowUserVariables = true,
                };

                if (MyInvocation.BoundParameters.ContainsKey("Database"))
                    builder.Database = Database;

                var connection = new MySqlConnection(builder.ConnectionString);
                connection.Open();

                if (MyInvocation.BoundParameters.ContainsKey("Database"))
                    _ = new MySqlCommand($"USE {Database}", connection);

                SessionState.PSVariable.Set(new PSVariable(
                    PSModuleConstants.ConnectionVariable,
                    connection,
                    ScopedItemOptions.Private
                ));
                WriteObject(connection);
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "DatabaseConnectionFailed", ErrorCategory.ConnectionError, null));
            }
        }
    }
}
