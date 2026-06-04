# ⚡ Quick Start - Icon Generation (5 Minutes)

Fast setup for developers who just want to generate Windows app icons.

## Prerequisites Check

```powershell
# Verify you have ImageMagick installed
convert --version
```

If that fails → [See Full Guide](ICON_GENERATION.md)

Note: PNG source format requires ImageMagick for proper resizing

---

## 🚀 One-Command Setup

### Option A: PowerShell (Recommended)

```powershell
cd C:\xampp\htdocs\EnterprisePOS\Platforms\Windows
.\generate-icons.ps1
```

### Option B: Batch Script

```cmd
cd C:\xampp\htdocs\EnterprisePOS\Platforms\Windows
generate-icons.bat
```

---

## ✅ You're Done!

Check `Assets/` folder - should have 7 PNG files:
- Logo.png (190×190)
- Square44x44Logo.png
- Square71x71Logo.png
- Square150x150Logo.png
- Square310x310Logo.png
- Wide310x150Logo.png
- SplashScreen.png (620×300)

---

## 🔄 Next Time

1. Edit: `Assets\source\appicon.png` (use any image editor)
2. Run: `.\generate-icons.ps1`
3. Done ✅

---

## ❌ If Icons Don't Generate

**Error 1:** `convert: command not found`
- Install ImageMagick: https://imagemagick.org
- Restart PowerShell after install
- Run `convert --version` to verify

**Error 2:** `appicon.svg not found`
- Verify file exists: `Assets\source\appicon.svg`
- Copy from: `..\..\Resources\AppIcon\appicon.svg`

**Error 3:** Generated PNG files are blurry
- Increase quality in script: change `-density 300` to `-density 600`

---

## 📚 Learn More

- [Complete Setup Guide](ICON_GENERATION.md)
- [Main README](README.md)
- [Icon Specifications](README.md#-icon-specifications)

---

**That's it! Your icons are ready for Windows packaging.**
