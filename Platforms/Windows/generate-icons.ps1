# ============================================================================
# Enterprise POS - Windows App Icon Generator (PowerShell)
# ============================================================================
# 
# Usage: .\generate-icons.ps1
# Prerequisites: ImageMagick must be installed
# Source: Assets\source\appicon.svg
# Output: 7 PNG files in Assets\
#
# ============================================================================

$ErrorActionPreference = 'Stop'

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourceImage = Join-Path $scriptPath "Assets\source\appicon.png"
$outputDir = Join-Path $scriptPath "Assets"

# Display header
Write-Host ""
Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host "  Enterprise POS - Windows App Icon Generator" -ForegroundColor Cyan
Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Source: Assets\source\appicon.png" -ForegroundColor DarkGray
Write-Host "Output: Assets\" -ForegroundColor DarkGray
Write-Host ""

# Verify source image exists
if (-not (Test-Path $sourceImage)) {
    Write-Host "[ERROR] Source image not found: $sourceImage" -ForegroundColor Red
    Write-Host "" 
    Write-Host "Please ensure appicon.png exists in: Assets\source\" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

Write-Host "[OK] Source image found" -ForegroundColor Green

# Icon specifications
$sizes = @(
    @{ name = "Logo.png"; size = "190x190"; desc = "Store logo" }
    @{ name = "Square44x44Logo.png"; size = "44x44"; desc = "Taskbar/Start menu" }
    @{ name = "Square71x71Logo.png"; size = "71x71"; desc = "File explorer" }
    @{ name = "Square150x150Logo.png"; size = "150x150"; desc = "Start menu tile" }
    @{ name = "Square310x310Logo.png"; size = "310x310"; desc = "Large tile" }
    @{ name = "Wide310x150Logo.png"; size = "310x150"; desc = "Wide tile" }
    @{ name = "SplashScreen.png"; size = "620x300"; desc = "Splash screen" }
)

# Check if ImageMagick is installed
$convertCmd = Get-Command convert -ErrorAction SilentlyContinue
if (-not $convertCmd) {
    Write-Host "[ERROR] ImageMagick not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Install ImageMagick (free, open source):" -ForegroundColor Yellow
    Write-Host "  https://imagemagick.org/script/download.php" -ForegroundColor DarkGray
    Write-Host ""
    Write-Host "After installation, restart PowerShell and run this script again." -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

Write-Host "[OK] ImageMagick found" -ForegroundColor Green
Write-Host ""

# Create output directory if needed
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
    Write-Host "[OK] Created output directory" -ForegroundColor Green
}

# Generate each icon
Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host "  Generating Icons..." -ForegroundColor Cyan
Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host ""

$errorCount = 0
foreach ($icon in $sizes) {
    $outputPath = Join-Path $outputDir $icon.name
    
    try {
        & convert $sourceImage -resize $icon.size -gravity center -extent $icon.size $outputPath
        Write-Host "  [OK] $($icon.name) ($($icon.size)) - $($icon.desc)" -ForegroundColor Green
    } catch {
        Write-Host "  [ERROR] Failed to generate $($icon.name): $_" -ForegroundColor Red
        $errorCount++
    }
}

# Summary
Write-Host ""
Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host "  Complete!" -ForegroundColor Cyan
Write-Host "============================================================================" -ForegroundColor Cyan
Write-Host ""

if ($errorCount -eq 0) {
    Write-Host "All icons generated successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Generated in: $outputDir" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "  1. Rebuild your app" -ForegroundColor DarkGray
    Write-Host "  2. Test on Windows" -ForegroundColor DarkGray
    Write-Host "  3. Verify icons display correctly" -ForegroundColor DarkGray
} else {
    Write-Host "Generated $($sizes.Count - $errorCount)/$($sizes.Count) icons" -ForegroundColor Yellow
    Write-Host "Errors: $errorCount" -ForegroundColor Red
    exit 1
}

Write-Host ""
