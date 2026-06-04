# Windows App Icons - Setup Guide

## 📁 Folder Structure

```
Platforms/Windows/Assets/
├── source/
│   └── appicon.svg          ← EDIT THIS to change all icons
├── Logo.png                 (190×190 - Store logo)
├── Square44x44Logo.png      (44×44 - Small tile)
├── Square71x71Logo.png      (71×71 - Medium)
├── Square150x150Logo.png    (150×150 - Large tile)
├── Square310x310Logo.png    (310×310 - Extra large)
├── Wide310x150Logo.png      (310×150 - Wide tile)
└── SplashScreen.png         (620×300 - Startup screen)
```

## 🎨 How to Update Icons

### Option 1: If ImageMagick is Installed
```powershell
cd Platforms\Windows
.\generate-icons.ps1
```

### Option 2: Manual Using Online Tools

1. **Convert SVG to PNG:**
   - Go to: https://svgtopng.com/
   - Upload: `Platforms/Windows/Assets/source/appicon.svg`
   - Export as PNG (620px wide for splash screen)

2. **Resize PNG to Each Size:**
   - Go to: https://bulkresizephotos.com/
   - Upload the PNG
   - Create these sizes and save each:
     - 44×44 → `Square44x44Logo.png`
     - 71×71 → `Square71x71Logo.png`
     - 150×150 → `Square150x150Logo.png`
     - 190×190 → `Logo.png`
     - 310×310 → `Square310x310Logo.png`
     - 310×150 → `Wide310x150Logo.png`
     - 620×300 → `SplashScreen.png`

3. **Save all to:** `Platforms/Windows/Assets/`

## ✅ Icon References in Code

- **Manifest:** [Platforms/Windows/Package.appxmanifest](Package.appxmanifest)
- **App Display Title:** [EnterprisePOS.csproj](../../EnterprisePOS.csproj) - `ApplicationTitle`

## 📝 File Sizes Reference

| File | Size | Usage |
|------|------|-------|
| Logo.png | 190×190 | Store listing |
| Square44x44Logo.png | 44×44 | Taskbar, start menu |
| Square71x71Logo.png | 71×71 | File explorer |
| Square150x150Logo.png | 150×150 | Start menu tile |
| Square310x310Logo.png | 310×310 | Large tile |
| Wide310x150Logo.png | 310×150 | Wide tile display |
| SplashScreen.png | 620×300 | App startup screen |

## 🔄 One-Step Icon Update Process

1. Edit `Assets/source/appicon.svg`
2. Run `generate-icons.ps1` OR use online tools
3. Save new PNGs to `Assets/`
4. Done! App will use new icons on next install

