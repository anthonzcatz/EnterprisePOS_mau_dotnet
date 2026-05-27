# Launch Enterprise POS on Windows (visible window).
# Do NOT use plain "dotnet run" if COREHOST_TRACE is set — it crashes silently.

$ErrorActionPreference = 'Stop'

foreach ($name in @('COREHOST_TRACE', 'COREHOST_TRACEFILE')) {
    [Environment]::SetEnvironmentVariable($name, $null, 'Process')
    Remove-Item "Env:$name" -ErrorAction SilentlyContinue
}

Set-Location $PSScriptRoot

Get-Process EnterprisePOS -ErrorAction SilentlyContinue | Stop-Process -Force

$framework = 'net10.0-windows10.0.19041.0'
$exe = Join-Path $PSScriptRoot "bin\Debug\$framework\win-x64\EnterprisePOS.exe"

Write-Host 'Building...' -ForegroundColor Cyan
dotnet build -f $framework
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

if (-not (Test-Path $exe)) {
    Write-Error "Exe not found: $exe"
}

Write-Host "Starting: $exe" -ForegroundColor Cyan
Write-Host 'Look for "Enterprise POS" on the taskbar or press Alt+Tab.' -ForegroundColor Yellow

$proc = Start-Process -FilePath $exe -WorkingDirectory (Split-Path $exe) -PassThru
Start-Sleep -Seconds 3

$running = Get-Process -Id $proc.Id -ErrorAction SilentlyContinue
if (-not $running) {
    Write-Host 'App exited immediately (crash). Check Windows Event Viewer > Application for EnterprisePOS.' -ForegroundColor Red
    exit 1
}

if ($running.MainWindowHandle -eq 0) {
    Write-Host "Process is running (PID $($proc.Id)) but no main window yet — wait or check taskbar." -ForegroundColor Yellow
} else {
    Write-Host "Window open: $($running.MainWindowTitle)" -ForegroundColor Green
}

Write-Host 'Close the app window to finish, or stop from Task Manager.' -ForegroundColor DarkGray
$running.WaitForExit()
exit $running.ExitCode
