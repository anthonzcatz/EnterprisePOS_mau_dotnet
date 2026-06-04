@echo off
REM ============================================================================
REM Enterprise POS - Windows App Icon Generator
REM Batch script for generating all required icon sizes from SVG source
REM ============================================================================
REM
REM Usage: generate-icons.bat
REM Prerequisites: ImageMagick must be installed and in PATH
REM Source: Assets\source\appicon.svg
REM Output: 7 PNG files in Assets\
REM
REM ============================================================================

setlocal enabledelayedexpansion

echo.
echo ============================================================================
echo  Enterprise POS - Icon Generator
echo ============================================================================
echo.

REM Get script directory
set "SCRIPT_DIR=%~dp0"
set "SOURCE_IMAGE=%SCRIPT_DIR%Assets\source\appicon.png"
set "OUTPUT_DIR=%SCRIPT_DIR%Assets"

REM Check if source image exists
if not exist "%SOURCE_IMAGE%" (
    echo [ERROR] Source image not found: %SOURCE_IMAGE%
    echo.
    echo Please ensure appicon.png exists in: Assets\source\
    echo.
    pause
    exit /b 1
)

echo [INFO] Source: %SOURCE_IMAGE%
echo [INFO] Output: %OUTPUT_DIR%
echo.

REM Check if convert.exe is available
where convert >nul 2>nul
if errorlevel 1 (
    echo [ERROR] ImageMagick is not installed or not in PATH
    echo.
    echo Please install ImageMagick from:
    echo   https://imagemagick.org/script/download.php
    echo.
    echo After installation, restart this command prompt and try again.
    echo.
    pause
    exit /b 1
)

echo [OK] ImageMagick found
echo.
echo ============================================================================
echo  Generating Icons...
echo ============================================================================
echo.

REM Icon sizes to generate
REM Format: size name
setlocal enabledelayedexpansion

set "ICONS[0]=190x190 Logo.png"
set "ICONS[1]=44x44 Square44x44Logo.png"
set "ICONS[2]=71x71 Square71x71Logo.png"
set "ICONS[3]=150x150 Square150x150Logo.png"
set "ICONS[4]=310x310 Square310x310Logo.png"
set "ICONS[5]=310x150 Wide310x150Logo.png"
set "ICONS[6]=620x300 SplashScreen.png"

REM Generate each icon
for /l %%i in (0,1,6) do (
    for /f "tokens=1,2" %%a in ("!ICONS[%%i]!") do (
        set "SIZE=%%a"
        set "FILENAME=%%b"
        set "OUTPUT_PATH=%OUTPUT_DIR%\!FILENAME!"
        
        echo Generating !FILENAME! (!SIZE!)...
        convert "%SOURCE_IMAGE%" -resize !SIZE! -gravity center -extent !SIZE! "!OUTPUT_PATH!"
        
        if errorlevel 1 (
            echo   [ERROR] Failed to generate !FILENAME!
        ) else (
            echo   [OK] Generated !FILENAME!
        )
    )
)

echo.
echo ============================================================================
echo  Icon Generation Complete!
echo ============================================================================
echo.
echo Generated files:
echo   - Logo.png (190x190)
echo   - Square44x44Logo.png (44x44)
echo   - Square71x71Logo.png (71x71)
echo   - Square150x150Logo.png (150x150)
echo   - Square310x310Logo.png (310x310)
echo   - Wide310x150Logo.png (310x150)
echo   - SplashScreen.png (620x300)
echo.
echo Location: %OUTPUT_DIR%
echo.
echo All icons are ready for Windows app packaging!
echo.
pause
