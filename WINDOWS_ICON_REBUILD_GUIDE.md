# 🔧 Windows App Icon - Rebuild & Deploy Guide

After updating PNG icons, you need to rebuild and reinstall the app for changes to take effect.

---

## 🚀 Step-by-Step Rebuild

### Step 1: Clean Build Artifacts

```powershell
cd C:\xampp\htdocs\EnterprisePOS

# Remove build outputs
dotnet clean --configuration Release
dotnet clean --configuration Debug

# Clear build cache
Remove-Item -Path "bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "obj" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path ".vs" -Recurse -Force -ErrorAction SilentlyContinue
```

### Step 2: Uninstall Old App Version

**Option A: Using Settings**
1. Open Windows Settings
2. Go to **Apps → Apps & features**
3. Search for **"Enterprise POS"**
4. Click → **Uninstall** → **Uninstall** (confirm)

**Option B: Using PowerShell (Administrator)**
```powershell
# List installed apps to find exact name
Get-AppxPackage | Where-Object {$_.Name -like "*enterprise*"} | Select-Object Name, Version

# Uninstall the app
Remove-AppxPackage -Package "EnterprisePOS_1.0.0.1_x64__<random-key>"
```

### Step 3: Rebuild Windows App

```powershell
cd C:\xampp\htdocs\EnterprisePOS

# Build for Windows (Release with self-contained runtime)
dotnet build -f net10.0-windows10.0.19041.0 -c Release

# Or use the build script if available
dotnet publish -f net10.0-windows10.0.19041.0 -c Release
```

Expected output:
```
Build succeeded
Generating .../EnterprisePOS.exe
```

### Step 4: Install New App Version

**Option A: Run EXE Directly**
```powershell
# Navigate to build output
cd bin\Release\net10.0-windows10.0.19041.0\win10-x64

# Run the application
.\EnterprisePOS.exe
```

**Option B: MSIX Installation (Recommended)**

1. **Create MSIX package:**
```powershell
cd C:\xampp\htdocs\EnterprisePOS

# Use publish script
.\publish-msix.ps1
```

2. **Install MSIX:**
   - Locate the generated `.msix` file in output folder
   - Double-click to install
   - Windows will automatically register the icons

**Option C: Command Line Installation**
```powershell
# Install MSIX using Add-AppxPackage
Add-AppxPackage -Path "path\to\EnterprisePOS_1.0.0.1_Release_Test.msix"
```

### Step 5: Verify Icons Display Correctly

1. **Check Taskbar:**
   - Look for "Enterprise POS" app in taskbar
   - Should show your purple bot icon (not .NET logo)

2. **Check Start Menu:**
   - Press Windows key
   - Search for "Enterprise POS"
   - Verify icon matches your design

3. **Check App Settings:**
   - Windows Settings → Apps → Apps & features
   - Find "Enterprise POS"
   - Should display with correct icon

---

## 🐛 Troubleshooting

### Issue: Old .NET icon still showing

**Solution:**
1. Hard delete the app:
```powershell
Remove-AppxPackage -Package "EnterprisePOS_1.0.0.1_x64__..." -PreserveApplicationData:$false
```

2. Clear cache:
```powershell
Remove-Item -Path "$env:LOCALAPPDATA\Packages\EnterprisePOS*" -Recurse -Force -ErrorAction SilentlyContinue
```

3. Rebuild and reinstall

###  Issue: App won't install

**Solution:**
```powershell
# Check if app is still registered
Get-AppxPackage -Name "*EnterprisePOS*"

# Register manifest manually
Add-AppxPackage -Path "Platforms\Windows\Package.appxmanifest"
```

### Issue: "File not found" error during installation

**Solution:**
- Verify all 7 PNG files exist in `Platforms/Windows/Assets/`
- Verify manifest paths are correct:
```xml
<Logo>Assets\Logo.png</Logo>
```

---

## 📋 Icon Files Verification Checklist

Before rebuilding, verify all files exist:

```powershell
# Check Windows Assets folder
Get-ChildItem -Path "Platforms\Windows\Assets\*.png" | Select-Object Name, Length

# Expected output (all files should exist):
# Logo.png
# Square44x44Logo.png
# Square71x71Logo.png
# Square150x150Logo.png
# Square310x310Logo.png
# Wide310x150Logo.png
# SplashScreen.png
```

---

## 🔄 Full Automated Rebuild Script

Save as `rebuild-with-icons.ps1`:

```powershell
# Automated rebuild script with icon verification

$ErrorActionPreference = 'Stop'

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Enterprise POS - Full Rebuild Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Verify icons
Write-Host "1. Verifying icon files..." -ForegroundColor Green
$iconFiles = @(
    "Logo.png",
    "Square44x44Logo.png",
    "Square71x71Logo.png",
    "Square150x150Logo.png",
    "Square310x310Logo.png",
    "Wide310x150Logo.png",
    "SplashScreen.png"
)

$assetsPath = "Platforms\Windows\Assets"
$missingFiles = @()

foreach ($file in $iconFiles) {
    $path = Join-Path $assetsPath $file
    if (Test-Path $path) {
        Write-Host "  ✓ $file" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $file" -ForegroundColor Red
        $missingFiles += $file
    }
}

if ($missingFiles.Count -gt 0) {
    Write-Host ""
    Write-Host "ERROR: Missing icon files: $missingFiles" -ForegroundColor Red
    Write-Host "Run generate-icons.ps1 first" -ForegroundColor Yellow
    exit 1
}

# Step 2: Clean build
Write-Host ""
Write-Host "2. Cleaning build artifacts..." -ForegroundColor Green
dotnet clean --configuration Release
Remove-Item -Path "bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "obj" -Recurse -Force -ErrorAction SilentlyContinue

# Step 3: Rebuild
Write-Host ""
Write-Host "3. Rebuilding application..." -ForegroundColor Green
dotnet build -f net10.0-windows10.0.19041.0 -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  ✅ Rebuild Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Uninstall old app from Settings" -ForegroundColor DarkGray
Write-Host "  2. Run: .\publish-msix.ps1" -ForegroundColor DarkGray
Write-Host "  3. Install the .msix package" -ForegroundColor DarkGray
Write-Host "  4. Verify icons in taskbar" -ForegroundColor DarkGray
Write-Host ""
```

**Run it:**
```powershell
.\rebuild-with-icons.ps1
```

---

## ✅ Success Indicators

After rebuild and reinstall, verify:

- ✅ Purple bot icon in taskbar (not .NET logo)
- ✅ Icon shows in Start menu
- ✅ App Settings displays correct icon
- ✅ All chart sections on Dashboard load and display
- ✅ POS page layout looks professional with proper colors
- ✅ Navigation sidebar is clean and responsive

---

##  Windows Build Settings Reference

**File:** `EnterprisePOS.csproj`

```xml
<!-- Windows Build Configuration -->
<PropertyGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">
  <WindowsPackageType>None</WindowsPackageType>
  <TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>
  <SupportedOSPlatformVersion>10.0.17763.0</SupportedOSPlatformVersion>
</PropertyGroup>
```

---

## 📞 Support

If icons still don't show after rebuild:

1. Check Task Manager → Details → verify `EnterprisePOS.exe` process
2. Check Event Viewer for app installation errors
3. Verify Windows version supports MSIX (Windows 10 Build 17763+)
4. Ensure user has admin rights for app installation
5. Try manual EXE execution in dev mode first before MSIX

---

**Status:** ✅ Ready for deployment  
**Last Updated:** June 2026  
**Version:** 1.0

