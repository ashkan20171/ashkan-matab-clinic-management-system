# Run only after verifying that the project archive came from a trusted source.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Get-ChildItem -LiteralPath $root -Recurse -File -Include *.resx,*.cs,*.config,*.dll,*.exe,*.sln,*.csproj | ForEach-Object {
    Unblock-File -LiteralPath $_.FullName
}
Write-Host 'Unblocked trusted Matab project files.' -ForegroundColor Green
Write-Host 'Reopen Matab.sln in Visual Studio 2022 and choose Rebuild Solution.'
Write-Warning 'Stimulsoft DLLs are not included. Install licensed compatible assemblies into Matab\lib to resolve Stimulsoft references.'
