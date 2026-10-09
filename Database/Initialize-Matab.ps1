param([string]$Server = '(local)')
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data
$root = Split-Path -Parent $PSScriptRoot
$sqlPath = Join-Path $PSScriptRoot 'Matab_Schema.sql'
$master = New-Object System.Data.SqlClient.SqlConnection("Data Source=$Server;Initial Catalog=master;Integrated Security=True;Connect Timeout=10;TrustServerCertificate=True")
try { $master.Open() } catch { Write-Error "Cannot connect to SQL Server '$Server'. Check instance and Windows authentication. $($_.Exception.Message)"; exit 1 }
$master.Close()
# GO batches are run individually; never run user input as SQL.
$connection = New-Object System.Data.SqlClient.SqlConnection("Data Source=$Server;Initial Catalog=master;Integrated Security=True;Connect Timeout=15;TrustServerCertificate=True")
$connection.Open()
try {
 $sql = Get-Content $sqlPath -Raw -Encoding UTF8
 foreach ($batch in ($sql -split '(?m)^GO\s*$')) {
  if (-not [string]::IsNullOrWhiteSpace($batch)) {
   $cmd=$connection.CreateCommand();$cmd.CommandText=$batch;$cmd.CommandTimeout=60;[void]$cmd.ExecuteNonQuery();$cmd.Dispose()
  }
 }
} finally { $connection.Close() }
$secret = Read-Host 'Choose a strong password for admin (input hidden)' -AsSecureString
$ptr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($secret)
try { $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ptr) } finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ptr) }
if ($password.Length -lt 12) { throw 'Admin password must contain at least 12 characters.' }
# Same PBKDF2 format as PasswordSecurity.cs
$salt = New-Object byte[] 16; [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($salt)
$derive = New-Object Security.Cryptography.Rfc2898DeriveBytes($password,$salt,120000)
$hash = $derive.GetBytes(32);$derive.Dispose();$password=$null
$encoded = 'PBKDF2$120000$'+[Convert]::ToBase64String($salt)+'$'+[Convert]::ToBase64String($hash)
$db=New-Object System.Data.SqlClient.SqlConnection("Data Source=$Server;Initial Catalog=Matab;Integrated Security=True;Connect Timeout=15;TrustServerCertificate=True")
$db.Open()
try {
 $cmd=$db.CreateCommand();$cmd.CommandText="IF EXISTS (SELECT 1 FROM dbo.Karbar WHERE UserName=@user) UPDATE dbo.Karbar SET Password=@pass, Access=N'admin' WHERE UserName=@user ELSE INSERT INTO dbo.Karbar (UserName,Password,Access) VALUES (@user,@pass,N'admin')"
 [void]$cmd.Parameters.Add('@user',[System.Data.SqlDbType]::NVarChar,256);$cmd.Parameters['@user'].Value='admin'
 [void]$cmd.Parameters.Add('@pass',[System.Data.SqlDbType]::NVarChar,256);$cmd.Parameters['@pass'].Value=$encoded
 [void]$cmd.ExecuteNonQuery();$cmd.Dispose()
} finally { $db.Close() }
$config=Join-Path $root 'Matab\app.config';[xml]$xml=Get-Content $config -Encoding UTF8
$xml.configuration.connectionStrings.add.connectionString="Data Source=$Server;Initial Catalog=Matab;Integrated Security=True;Connect Timeout=15;TrustServerCertificate=True"
$xml.Save($config)
Write-Host 'Database created; admin account configured; app.config updated.' -ForegroundColor Green
Write-Host 'Login: admin / your chosen password / role: Manager (مدیر)'
