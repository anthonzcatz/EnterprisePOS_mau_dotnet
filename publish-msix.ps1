# Build signed MSIX installer (Release) for local/desktop install.
# Debug builds stay WindowsPackageType=None — only this script produces MSIX.

$ErrorActionPreference = 'Stop'

foreach ($name in @('COREHOST_TRACE', 'COREHOST_TRACEFILE')) {
    [Environment]::SetEnvironmentVariable($name, $null, 'Process')
    Remove-Item "Env:$name" -ErrorAction SilentlyContinue
}

Set-Location $PSScriptRoot

Get-Process EnterprisePOS -ErrorAction SilentlyContinue | Stop-Process -Force

$framework = 'net10.0-windows10.0.19041.0'
# Default output paths (after publish-msix.ps1)
$script:DefaultMsixPath = Join-Path $PSScriptRoot "bin\Release\$framework\win-x64\AppPackages\EnterprisePOS_1.0.0.1_Test\EnterprisePOS_1.0.0.1_x64.msix"
$script:DefaultSideloadFolder = Join-Path $PSScriptRoot "bin\Release\$framework\win-x64\AppPackages\EnterprisePOS_1.0.0.1_Test"
$certDir = Join-Path $PSScriptRoot 'certs'
$certPath = Join-Path $certDir 'EnterprisePOS.pfx'
$certPassword = 'EnterprisePOS-Dev'
$certSubject = 'CN=EnterprisePOS'
$displayVersion = '1.0.1'

if (-not (Test-Path $certDir)) {
    New-Item -ItemType Directory -Path $certDir | Out-Null
}

function Ensure-AppxDevCertificate {
    $storeCert = Get-ChildItem -Path Cert:\CurrentUser\My |
        Where-Object { $_.Subject -eq $certSubject } |
        Sort-Object NotAfter -Descending |
        Select-Object -First 1

    if (-not $storeCert) {
        Write-Host 'Creating MSIX publisher certificate (one-time)...' -ForegroundColor Cyan
        $storeCert = New-SelfSignedCertificate `
            -Type Custom `
            -Subject $certSubject `
            -KeyUsage DigitalSignature `
            -FriendlyName 'Enterprise POS Dev' `
            -CertStoreLocation 'Cert:\CurrentUser\My' `
            -KeySpec Signature `
            -TextExtension @(
                '2.5.29.37={text}1.3.6.1.5.5.7.3.3'
                '2.5.29.19={text}'
            )
    }

    $secure = ConvertTo-SecureString -String $certPassword -Force -AsPlainText
    Export-PfxCertificate -Cert $storeCert -FilePath $certPath -Password $secure -Force | Out-Null
    $cerPath = Join-Path $certDir 'EnterprisePOS.cer'
    Export-Certificate -Cert $storeCert -FilePath $cerPath -Force | Out-Null
    Write-Host "Certificate thumbprint: $($storeCert.Thumbprint)" -ForegroundColor DarkGray
    return $storeCert
}

$cert = Ensure-AppxDevCertificate

Write-Host 'Publishing MSIX (Release)...' -ForegroundColor Cyan
$sw = [System.Diagnostics.Stopwatch]::StartNew()

# Do not pass -r win-x64 on multi-target MAUI projects (breaks restore with Mono RID).
dotnet publish `
    -c Release `
    -f $framework `
    -p:WindowsPackageType=MSIX `
    -p:ApplicationDisplayVersion=$displayVersion `
    -p:AppxPackageSigningEnabled=true `
    -p:PackageCertificateThumbprint=$($cert.Thumbprint) `
    -p:GenerateAppxSymbolPackage=false `
    -v minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host 'MSIX publish failed.' -ForegroundColor Red
    exit $LASTEXITCODE
}

$sw.Stop()
Write-Host ("Publish finished in {0:n1}s" -f $sw.Elapsed.TotalSeconds) -ForegroundColor Green

$appPackagesRoot = Join-Path $PSScriptRoot "bin\Release\$framework\win-x64\AppPackages"
$msix = $null
if (Test-Path $appPackagesRoot) {
    $msix = Get-ChildItem -Path $appPackagesRoot -Filter '*.msix' -Recurse -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1
}

if (-not $msix) {
    Write-Host "Publish succeeded but no .msix found under: $appPackagesRoot" -ForegroundColor Yellow
    exit 0
}

# Sideload script expects .cer next to .msix (Developer Mode install).
$testDirs = Get-ChildItem -Path $appPackagesRoot -Directory -Filter '*_Test' -ErrorAction SilentlyContinue
$cerPath = Join-Path $certDir 'EnterprisePOS.cer'
foreach ($testDir in $testDirs) {
    if (Test-Path $cerPath) {
        Copy-Item $cerPath (Join-Path $testDir.FullName 'EnterprisePOS.cer') -Force
    }
}

Write-Host ''
Write-Host 'MSIX installer file:' -ForegroundColor Cyan
Write-Host $msix.FullName
Write-Host ("Size: {0:N2} MB" -f ($msix.Length / 1MB))
Write-Host ''
Write-Host 'Sideload folder (MSIX + Install.ps1 + .cer):' -ForegroundColor Cyan
Write-Host (Split-Path $msix.FullName -Parent)
Write-Host ''
Write-Host 'Quick copy path (default build):' -ForegroundColor DarkGray
Write-Host $DefaultMsixPath
Write-Host ''
Write-Host 'Install (Developer Mode ON) + Desktop shortcut:' -ForegroundColor Yellow
Write-Host '  .\install-msix.ps1'
Write-Host ''
Write-Host 'Desktop shortcut only (already installed):' -ForegroundColor DarkGray
Write-Host '  .\create-desktop-shortcut.ps1'
