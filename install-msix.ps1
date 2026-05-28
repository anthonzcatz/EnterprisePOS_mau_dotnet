# Install Enterprise POS MSIX with Developer Mode + one-time cert trust (UAC).
# 1) Turn ON: Settings > System > For developers > Developer Mode
# 2) Run: .\publish-msix.ps1  then  .\install-msix.ps1

param(
    [string]$TestFolder,
    [switch]$SkipCertTrust
)

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

$framework = 'net10.0-windows10.0.19041.0'
$certSubject = 'CN=EnterprisePOS'

function Test-DeveloperModeEnabled {
    $key = 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock'
    if (-not (Test-Path $key)) { return $false }
    return (Get-ItemProperty -Path $key).AllowDevelopmentWithoutDevLicense -eq 1
}

function Resolve-TestFolder {
    if ($TestFolder) {
        if (-not (Test-Path $TestFolder)) { throw "Folder not found: $TestFolder" }
        return (Resolve-Path $TestFolder).Path
    }

    $appPackagesRoot = Join-Path $PSScriptRoot "bin\Release\$framework\win-x64\AppPackages"
    if (-not (Test-Path $appPackagesRoot)) {
        throw "No AppPackages folder. Run .\publish-msix.ps1 first."
    }

    foreach ($folder in (Get-ChildItem -Path $appPackagesRoot -Directory -Filter '*_Test' | Sort-Object LastWriteTime -Descending)) {
        $msix = Get-ChildItem -Path $folder.FullName -Filter '*.msix' -ErrorAction SilentlyContinue | Select-Object -First 1
        if (-not $msix) { continue }
        if ((Get-AuthenticodeSignature -FilePath $msix.FullName).Status -eq 'Valid') {
            return $folder.FullName
        }
        Write-Host "Skipping unsigned: $($folder.Name)" -ForegroundColor DarkYellow
    }

    throw "No signed MSIX. Run .\publish-msix.ps1"
}

function Get-CerPath {
    param([string]$Folder)

    $cerInFolder = Join-Path $Folder 'EnterprisePOS.cer'
    $repoCer = Join-Path $PSScriptRoot 'certs\EnterprisePOS.cer'

    if (Test-Path $repoCer) {
        Copy-Item $repoCer $cerInFolder -Force
    } elseif (-not (Test-Path $cerInFolder)) {
        $storeCert = Get-ChildItem Cert:\CurrentUser\My |
            Where-Object { $_.Subject -eq $certSubject } |
            Sort-Object NotAfter -Descending |
            Select-Object -First 1
        if (-not $storeCert) { throw 'No certificate. Run .\publish-msix.ps1 first.' }
        Export-Certificate -Cert $storeCert -FilePath $cerInFolder -Force | Out-Null
    }

    return $cerInFolder
}

function Install-TrustedRootCertificate {
    param([string]$CerPath)

    Write-Host ''
    Write-Host 'Next: Windows will show UAC —' -ForegroundColor Yellow
    Write-Host '  "Do you want to allow this app to make changes to your device?"' -ForegroundColor Yellow
    Write-Host '  App shown: certutil.exe (or Certificate Utility)' -ForegroundColor Yellow
    Write-Host '  This is safe: it only adds YOUR dev signing cert (EnterprisePOS) to Trusted Root.' -ForegroundColor Yellow
    Write-Host '  Click Yes once per PC, then install continues.' -ForegroundColor Yellow
    Write-Host ''
    Start-Sleep -Seconds 2

    $proc = Start-Process -FilePath 'certutil.exe' `
        -ArgumentList @('-addstore', 'Root', $CerPath) `
        -Verb RunAs `
        -Wait `
        -PassThru

    if ($proc.ExitCode -ne 0) {
        throw "certutil failed ($($proc.ExitCode)). Install $CerPath manually into Trusted Root."
    }

    Write-Host 'Certificate added to Local Machine > Trusted Root.' -ForegroundColor Green
}

if (-not (Test-DeveloperModeEnabled)) {
    Write-Host 'Developer Mode is OFF.' -ForegroundColor Red
    Write-Host 'Settings > System > For developers > Developer Mode' -ForegroundColor Yellow
    exit 1
}

Write-Host 'Developer Mode: ON' -ForegroundColor Green

$testDir = Resolve-TestFolder
$cerPath = Get-CerPath -Folder $testDir

if (-not $SkipCertTrust) {
    Install-TrustedRootCertificate -CerPath $cerPath
}

Get-Process EnterprisePOS -ErrorAction SilentlyContinue | Stop-Process -Force

$installScript = Join-Path $testDir 'Install.ps1'
if (-not (Test-Path $installScript)) { throw "Missing Install.ps1 in $testDir" }

Write-Host ''
Write-Host "Installing from: $testDir" -ForegroundColor Cyan
Set-Location $testDir
& $installScript

if ($LASTEXITCODE -and $LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Set-Location $PSScriptRoot
$shortcutScript = Join-Path $PSScriptRoot 'create-desktop-shortcut.ps1'
if (Test-Path $shortcutScript) {
    Write-Host ''
    & $shortcutScript
}

Write-Host ''
Write-Host 'Installed. Launch from Desktop shortcut or Start menu ("Enterprise POS").' -ForegroundColor Green
