# 🎨 Enterprise POS - Windows App Icons

Complete, self-contained icon management system for Windows MSIX deployment.

## 📁 Folder Structure

```
Assets/
├── README.md                 ← You are here
├── source/
│   ├── appicon.png          ← PRIMARY SOURCE (edit this only)
│   └── template.md          ← Icon guidelines
│
├── ICON_GENERATION.md       ← Complete setup guide
├── QUICK_START.md           ← Fast setup for developers
│
├── Generated Icons (Do not edit):
├── Logo.png                 (190×190 - Store logo)
├── Square44x44Logo.png      (44×44 - Taskbar/Start Menu)
├── Square71x71Logo.png      (71×71 - File Explorer)
├── Square150x150Logo.png    (150×150 - Start Menu Tile)
├── Square310x310Logo.png    (310×310 - Extra Large Tile)
├── Wide310x150Logo.png      (310×150 - Wide Tile)
└── SplashScreen.png         (620×300 - App Startup Screen)
```

## ⚡ Quick Start (2 minutes)

### If You Have ImageMagick Installed
```powershell
cd Platforms\Windows
.\generate-icons.ps1
```

### If You Don't Have ImageMagick
1. [Download ImageMagick (FREE)](https://imagemagick.org/script/download.php) - Open source, works offline
2. Install with default settings
3. Run the script above

Note: PNG source format requires ImageMagick (works better than SVG for resizing PNG)

## 🛠️ Complete Setup Guide

See [ICON_GENERATION.md](ICON_GENERATION.md) for:
- Step-by-step ImageMagick installation
- Batch script alternative
- Manual PNG generation using free tools
- Troubleshooting

## 📝 Icon Specifications

| File | Size | DPI | Usage | Transparency |
|------|------|-----|-------|--------------|
| **Logo.png** | 190×190 | 96 | Store listing logo | Yes (PNG) |
| **Square44x44Logo.png** | 44×44 | 96 | Taskbar, Start menu | Yes (PNG) |
| **Square71x71Logo.png** | 71×71 | 96 | File explorer view | Yes (PNG) |
| **Square150x150Logo.png** | 150×150 | 96 | Start menu tile | Yes (PNG) |
| **Square310x310Logo.png** | 310×310 | 96 | Large tile display | Yes (PNG) |
| **Wide310x150Logo.png** | 310×150 | 96 | 2:1 wide tile | Yes (PNG) |
| **SplashScreen.png** | 620×300 | 96 | App startup screen | Yes (PNG) |

## 🔄 Icon Update Workflow

### Updating Icons (Recommended)

1. **Edit source PNG:**
   ```
   source/appicon.png  ← Make changes here (edit with Paint, Photoshop, GIMP, etc.)
   ```

2. **Generate all sizes:**
   ```powershell
   .\generate-icons.ps1  # Windows PowerShell
   # OR
   .\generate-icons.bat  # Windows Batch
   ```

3. **Verify output:**
   - Check all 7 PNG files in this folder
   - Look at actual app on taskbar to verify

4. **Commit & Deploy:**
   - Git commit the new PNGs
   - Build & publish your app

### Without ImageMagick (Fallback)

See manual instructions in [ICON_GENERATION.md](ICON_GENERATION.md)

## 🎯 References

**Related Files:**
- Manifest: `Package.appxmanifest` - References these icons
- Project: `EnterprisePOS.csproj` - ApplicationTitle and DisplayVersion
- Source PNG: `source/appicon.png` - Edit this to change icons

**Windows Documentation:**
- [MSIX App Icon Requirements](https://docs.microsoft.com/windows/uwp/design/app-icons-and-logos)
- [App Manifest Schema](https://docs.microsoft.com/windows/win32/appxpkg/appx-manifest-elements)

## 💡 Best Practices

✅ **DO:**
- Keep source PNG in `source/` folder
- Use meaningful file names (clear sizes in name)
- Edit PNG with professional tools (GIMP, Photoshop, Paint.NET)
- Test icons on actual app before deployment
- Save source at high resolution (1000×1000 px or larger)

❌ **DON'T:**
- Manually edit generated PNG files
- Use low-quality source images
- Mix different icon styles
- Hardcode icon paths in code
- Edit individual sizes (regenerate instead)

## 🔧 Troubleshooting

**Icons not showing in installed app?**
- Rebuild app: `dotnet clean && dotnet build`
- Uninstall old version first
- Check Package.appxmanifest paths

**ImageMagick not found?**
- Install from: https://imagemagick.org
- Restart terminal after install
- Verify: `convert --version`

**Generated images look blurry?**
- Increase density in script: `-density 300` → `-density 600`
- Use vector SVG source (scalable)
- Keep source 1000px+ for quality

## 📚 Resources

- **SVG Editing:** Inkscape (free), Adobe XD, Figma
- **PNG Viewing:** Any image viewer, Windows Photos app
- **ImageMagick:** Open source, free, works offline
- **GIMP:** Free alternative for image editing

---

**Last Updated:** June 2026  
**Status:** ✅ Production Ready  
**Maintenance:** Batch update - regenerate all 7 sizes when source changes
