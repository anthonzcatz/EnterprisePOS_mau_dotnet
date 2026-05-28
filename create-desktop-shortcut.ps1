# Desktop shortcut for Enterprise POS (MSIX) — launches the app, not a folder.

$ErrorActionPreference = 'Stop'

$appTitle = 'Dos Avenue'
$shortcutName = "$appTitle.lnk"

function Get-InstalledEnterprisePos {
    $pkg = Get-AppxPackage -Name '*EnterprisePOS*' -ErrorAction SilentlyContinue |
        Sort-Object { [Version]$_.Version } -Descending |
        Select-Object -First 1

    if (-not $pkg) { return $null }

    $exePath = Join-Path $pkg.InstallLocation 'EnterprisePOS.exe'
    $fromStart = Get-StartApps | Where-Object {
        $_.Name -eq $appTitle -or $_.Name -eq 'Enterprise POS' -or $_.Name -eq 'EnterprisePOS'
    } | Select-Object -First 1
    $appId = if ($fromStart) { $fromStart.AppID } else { "$($pkg.PackageFamilyName)!App" }

    return [PSCustomObject]@{
        ExePath = if (Test-Path $exePath) { $exePath } else { $null }
        AppId   = $appId
    }
}

function New-EnterprisePosShortcut {
    param(
        [string]$DesktopFolder,
        [string]$AppId,
        [string]$ExePath
    )

    $lnkPath = Join-Path $DesktopFolder $shortcutName
    $shell = New-Object -ComObject WScript.Shell
    $shortcut = $shell.CreateShortcut($lnkPath)

    # Do NOT use explorer.exe shell:AppsFolder — that opens a folder view, not the app.
    $shortcut.TargetPath = "$env:SystemRoot\System32\cmd.exe"
    $shortcut.Arguments = "/c start `"`" `"shell:AppsFolder\$AppId`""
    $shortcut.WorkingDirectory = $env:SystemRoot
    $shortcut.WindowStyle = 7   # Minimized — hide brief cmd flash

    if ($ExePath) {
        $shortcut.IconLocation = "$ExePath,0"
    }

    $shortcut.Description = 'Dos Avenue Coffee POS'
    $shortcut.Save()
    return $lnkPath
}

$installed = Get-InstalledEnterprisePos
if (-not $installed) {
    Write-Host 'Dos Avenue is not installed. Run: .\install-msix.ps1' -ForegroundColor Red
    exit 1
}

$desktop = [Environment]::GetFolderPath('Desktop')
if (-not (Test-Path $desktop)) {
    $desktop = Join-Path $env:USERPROFILE 'Desktop'
}

$lnk = New-EnterprisePosShortcut -DesktopFolder $desktop -AppId $installed.AppId -ExePath $installed.ExePath

# Remove old .bat launcher if present (often confused with the real shortcut)
$oldBat = Join-Path $desktop "$appTitle.bat"
if (Test-Path $oldBat) {
    Remove-Item $oldBat -Force
    Write-Host "Removed old launcher: $oldBat" -ForegroundColor DarkGray
}

Write-Host 'Desktop shortcut fixed:' -ForegroundColor Green
Write-Host $lnk
Write-Host 'Double-click "Dos Avenue.lnk" on your Desktop.' -ForegroundColor Yellow
