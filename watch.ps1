# Live dev loop: rebuild + restart on file changes (Debug unpackaged app).
# Usage: .\watch.ps1

$ErrorActionPreference = 'Stop'

foreach ($name in @('COREHOST_TRACE', 'COREHOST_TRACEFILE')) {
    [Environment]::SetEnvironmentVariable($name, $null, 'Process')
    Remove-Item "Env:$name" -ErrorAction SilentlyContinue
}

Set-Location $PSScriptRoot

$framework = 'net10.0-windows10.0.19041.0'

Write-Host 'Dos Avenue POS - dotnet watch (Debug)' -ForegroundColor Cyan
Write-Host '  Save .xaml / .cs files to rebuild and restart the app.' -ForegroundColor DarkGray
Write-Host '  For instant XAML updates use Visual Studio Hot Reload (F5).' -ForegroundColor DarkGray
Write-Host '  MSIX on Desktop is NOT live preview - use this script instead.' -ForegroundColor DarkGray
Write-Host '  Press Ctrl+C to stop.' -ForegroundColor DarkGray
Write-Host ''

Get-Process EnterprisePOS -ErrorAction SilentlyContinue | Stop-Process -Force

dotnet watch run --project EnterprisePOS.csproj -f $framework
