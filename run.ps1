# Launch EnterprisePOS on Windows desktop.
# Usage: .\run.ps1

$ErrorActionPreference = 'Stop'

Remove-Item Env:COREHOST_TRACE -ErrorAction SilentlyContinue
Remove-Item Env:COREHOST_TRACEFILE -ErrorAction SilentlyContinue

Set-Location $PSScriptRoot

Get-Process EnterprisePOS -ErrorAction SilentlyContinue | Stop-Process -Force

$framework = 'net10.0-windows10.0.19041.0'
$exe = Join-Path $PSScriptRoot "bin\Debug\$framework\win-x64\EnterprisePOS.exe"

Write-Host 'Building...' -ForegroundColor Cyan
dotnet build -f $framework -c Debug
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

if (-not (Test-Path $exe)) {
    Write-Error "Exe not found: $exe"
    exit 1
}

Write-Host "Launching: $exe" -ForegroundColor Cyan
Write-Host 'Check your taskbar for the EnterprisePOS window.' -ForegroundColor Yellow

$proc = Start-Process -FilePath $exe -WorkingDirectory (Split-Path $exe) -PassThru
Start-Sleep -Seconds 4

$running = Get-Process -Id $proc.Id -ErrorAction SilentlyContinue
if (-not $running) {
    Write-Host 'App crashed on startup. Check Event Viewer > Windows Logs > Application.' -ForegroundColor Red
    exit 1
}

Write-Host "App is running (PID $($proc.Id))." -ForegroundColor Green
Write-Host 'Waiting for app to close...' -ForegroundColor DarkGray
$running.WaitForExit()
exit $running.ExitCode
