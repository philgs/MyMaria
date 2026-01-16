@{
    RootModule           = 'MyMaria.dll'
    ModuleVersion        = '1.0.0'
    Author               = 'Phil Silva'
    GUID                 = 'c07946ac-31e8-4940-ad27-dd535b06c8af'
    Description          = 'A simple PowerShell module for working with MySQL and MariaDB'
    PowerShellVersion    = '7.0'
    CompatiblePSEditions = @('Core')
    AliasesToExport      = @(
        'cnmydb'
        'dcmydb'
        'imyqry'
        'imycmd'
    )
    FunctionsToExport    = @()
    CmdletsToExport      = @(
        'Connect-Database'
        'Disconnect-Database'
        'Invoke-NonQuery'
        'Invoke-Query'
    )
    VariablesToExport    = @(
        'MyMariaConnection'
    )
    PrivateData          = @{
        PSData = @{
            Tags       = @(
                'database'
                'mariadb'
                'mysql'
                'sql'
            )
            ProjectUri = 'https://github.com/philgs/MyMaria'
            LicenseUri = 'https://github.com/philgs/MyMaria/blob/main/LICENSE'
        }
    }
}
