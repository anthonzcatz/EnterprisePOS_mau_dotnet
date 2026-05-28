# Windows build and installer guide

How to run Enterprise POS during development, publish a portable folder, or generate a signed **MSIX** installer for Windows desktop.

## Installer file locations (quick reference)

| Item | Path |
|------|------|
| **MSIX installer** | `c:\xampp\htdocs\EnterprisePOS\bin\Release\net10.0-windows10.0.19041.0\win-x64\AppPackages\EnterprisePOS_1.0.0.0_Test\EnterprisePOS_1.0.0.0_x64.msix` |
| **Sideload folder** | `c:\xampp\htdocs\EnterprisePOS\bin\Release\net10.0-windows10.0.19041.0\win-x64\AppPackages\EnterprisePOS_1.0.0.0_Test\` |
| **Signing cert (dev)** | `c:\xampp\htdocs\EnterprisePOS\certs\EnterprisePOS.cer` |

Open the sideload folder in Explorer:

```powershell
explorer "c:\xampp\htdocs\EnterprisePOS\bin\Release\net10.0-windows10.0.19041.0\win-x64\AppPackages\EnterprisePOS_1.0.0.0_Test"
```

After `.\publish-msix.ps1`, the console prints the exact `.msix` path (version folder may change when you bump `$displayVersion`).

---

## Where the app goes after install (MSIX)

MSIX does **not** install into your project `bin` folder. Windows copies the app here:

| What | Path |
|------|------|
| **Installed app (program files)** | `C:\Program Files\WindowsApps\EnterprisePOS_1.0.0.0_x64__5rf86ty3rfgc4\` |
| **Per-user app data** | `%LOCALAPPDATA%\Packages\EnterprisePOS_5rf86ty3rfgc4\` |
| **Desktop shortcut** | `%USERPROFILE%\Desktop\Enterprise POS.lnk` |
| **Start menu** | Search **Enterprise POS** |

The `WindowsApps` folder is protected — do not edit or run `.exe` from there directly. Always launch via **Desktop shortcut** or **Start menu**.

**Installer source (unchanged after install)** — your built `.msix` stays in the repo:

`c:\xampp\htdocs\EnterprisePOS\bin\Release\net10.0-windows10.0.19041.0\win-x64\AppPackages\EnterprisePOS_1.0.0.0_Test\EnterprisePOS_1.0.0.0_x64.msix`

Check install location anytime:

```powershell
Get-AppxPackage -Name '*EnterprisePOS*' | Select-Object Name, Version, InstallLocation
```

Uninstall:

```powershell
Remove-AppxPackage -Package (Get-AppxPackage -Name '*EnterprisePOS*').PackageFullName
```

---

## Prerequisites

| Requirement | Notes |
|-------------|--------|
| Windows 10/11 (x64) | Target OS for the desktop app |
| [.NET SDK](https://dotnet.microsoft.com/download) | Same major version as the project (`net10.0`) |
| MAUI workload | `dotnet workload install maui` |
| Windows App SDK / MSIX tooling | Usually included with the MAUI Windows workload |

Verify from the repo root:

```powershell
dotnet --version
dotnet workload list
```

## Real-time preview while developing (standard workflow)

| Goal | Standard approach | Command / tool |
|------|-------------------|----------------|
| **See UI changes quickly** | **XAML Hot Reload** (no full restart) | Visual Studio 2022 → F5 → edit `.xaml` → save (Hot Reload toolbar) |
| **Auto rebuild on save** | **`dotnet watch`** | `.\watch.ps1` |
| **One-off run** | Debug build + launch | `.\run.ps1` |
| **Test real installer** | MSIX (slow loop) | `.\publish-msix.ps1` then `.\install-msix.ps1` |

**Important:** Live preview uses the **unpackaged Debug** app (`bin\Debug\...`), **not** the MSIX on Desktop. After MSIX install, code changes do **not** appear until you publish and install again.

### Recommended daily loop (Cursor / terminal)

1. Start watch once and leave it running:

```powershell
.\watch.ps1
```

Or double-click **`watch.cmd`** if PowerShell blocks scripts. (Uses ASCII-only text so it runs on all Windows locales.)

2. Edit `Views\POSPage.xaml`, `Components\*.xaml`, `ViewModels\*.cs`, etc.
3. Save — watch rebuilds and restarts the window (usually a few seconds).

### Fastest UI iteration (Visual Studio — industry standard for MAUI)

1. Open `EnterprisePOS.sln` in **Visual Studio 2022** with **.NET MAUI** workload.
2. Set target: **Windows Machine** / `net10.0-windows10.0.19041.0`.
3. Press **F5** (Debug).
4. Enable **XAML Hot Reload** and **.NET Hot Reload** (default in recent VS).
5. Change XAML → **Save** → UI updates **without** closing the app (for most layout/style changes).

Large changes (new pages, DI, some C#) may still need a full restart — watch or F5 again.

### What Hot Reload cannot do reliably

- Replace MSIX-installed app on Desktop automatically
- Some C# refactors (rename class, change constructors) — full rebuild required
- Manifest / package identity changes — use `publish-msix.ps1` again

---

## Development run (one-off)

Use the launcher script so `COREHOST_TRACE` is cleared and the app is built before start:

```powershell
.\run.ps1
```

- Output: `bin\Debug\net10.0-windows10.0.19041.0\win-x64\EnterprisePOS.exe`
- Close any running `EnterprisePOS` process before rebuilding.

**Do not** leave `COREHOST_TRACE` or `COREHOST_TRACEFILE` set in system/user environment variables — the app can exit immediately on Windows.

## Project packaging defaults

In [`EnterprisePOS.csproj`](../EnterprisePOS.csproj):

- `WindowsPackageType` is **`None`** for normal Debug/Release builds.
- MSIX is produced **only** when you publish with `-p:WindowsPackageType=MSIX` (see below).

This keeps day-to-day builds fast and avoids changing the dev workflow.

---

## Option A — Portable folder (no installer)

Self-contained output you can zip and copy to another PC (no MSIX, no Store).

```powershell
dotnet publish -c Release -f net10.0-windows10.0.19041.0 -p:WindowsPackageType=None
```

Typical output:

`bin\Release\net10.0-windows10.0.19041.0\win-x64\publish\`

Run `EnterprisePOS.exe` from that folder on the target machine (same architecture: x64).

---

## Option B — MSIX installer (recommended for desktop install)

### Quick path (script)

From the repo root:

```powershell
.\publish-msix.ps1
```

The script:

1. Clears `COREHOST_TRACE` for the session
2. Stops any running `EnterprisePOS` process
3. Creates a **one-time** dev signing cert at `certs\EnterprisePOS.pfx` (gitignored)
4. Publishes a **signed** Release MSIX
5. Prints the `.msix` path and install command

**First run** can take several minutes (full Release + packaging). Later runs are faster if you do not `dotnet clean`.

### Where the MSIX file is

Default signed installer (version `1.0.0.0`):

```
c:\xampp\htdocs\EnterprisePOS\bin\Release\net10.0-windows10.0.19041.0\win-x64\AppPackages\EnterprisePOS_1.0.0.0_Test\EnterprisePOS_1.0.0.0_x64.msix
```

Parent folder also contains `Install.ps1`, `EnterprisePOS.cer`, and `Dependencies\` for sideloading.

### Install the MSIX (Developer Mode — recommended for dev)

1. Turn on **Settings → System → For developers → Developer Mode**.
2. From the repo root:

```powershell
.\publish-msix.ps1
.\install-msix.ps1
```

`install-msix.ps1`:

- Checks Developer Mode
- Shows **UAC** for **certutil.exe** — click **Yes** once (Trusted Root)
- Runs `Install.ps1` from the `*_Test` folder
- Creates a **Desktop shortcut** (`Enterprise POS.lnk`) via [`create-desktop-shortcut.ps1`](../create-desktop-shortcut.ps1)

Optional — specific test folder:

```powershell
.\install-msix.ps1 -TestFolder "bin\Release\net10.0-windows10.0.19041.0\win-x64\AppPackages\EnterprisePOS_1.0.0.0_Test"
```

**Do not** double-click the `.msix` alone on a self-signed build — you may get **0x800B010A**. Use `install-msix.ps1` instead.

### Desktop shortcut / EXE on Desktop

MSIX does **not** copy a loose `.exe` to Desktop automatically. The real program file is:

```
C:\Program Files\WindowsApps\EnterprisePOS_1.0.0.0_x64__5rf86ty3rfgc4\EnterprisePOS.exe
```

(That folder is protected — use a shortcut, not a manual copy.)

| When | Command |
|------|---------|
| After install | Automatic — `install-msix.ps1` calls `create-desktop-shortcut.ps1` |
| Nothing on Desktop / need EXE icon | `.\create-desktop-shortcut.ps1` |

This creates `Enterprise POS.lnk` on your Desktop (launches the app via `cmd /c start shell:AppsFolder\...` — **not** `explorer.exe`, which only opens a folder view).

If double-click opens a **folder** instead of the app, run `.\create-desktop-shortcut.ps1` again to repair the shortcut.

Paths: `%USERPROFILE%\Desktop\` (and OneDrive Desktop if you use OneDrive sync).

If you still do not see icons, press **F5** on the Desktop or open Desktop in Explorer:

```powershell
explorer $env:USERPROFILE\Desktop
```

**Want a normal folder with `EnterprisePOS.exe` you can zip?** Use portable publish (Option A) — not MSIX.

### Error 0x800B010A / 0x800B0109 (certificate not verified)

**Developer Mode alone is not enough** for a self-signed MSIX. You still need the publisher cert in **Trusted Root** once per machine.

Use `.\install-msix.ps1` and click **Yes** on the UAC prompt (runs `certutil -addstore Root`).

**Manual trust (alternative):** double-click `certs\EnterprisePOS.cer` → **Install Certificate** → **Local Machine** → **Trusted Root Certification Authorities** → then `.\install-msix.ps1 -SkipCertTrust`.

For production/Store distribution, use a proper code-signing certificate and update publisher identity in [`Platforms/Windows/Package.appxmanifest`](../Platforms/Windows/Package.appxmanifest).

### Manual MSIX publish (same as the script)

```powershell
dotnet publish -c Release -f net10.0-windows10.0.19041.0 `
  -p:WindowsPackageType=MSIX `
  -p:ApplicationDisplayVersion=1.0.0 `
  -p:AppxPackageSigningEnabled=true `
  -p:PackageCertificateThumbprint=<thumbprint-from-cert-store> `
  -p:GenerateAppxSymbolPackage=false
```

Run `publish-msix.ps1` once to create the dev cert in `Cert:\CurrentUser\My` (subject `CN=EnterprisePOS`, matches the manifest). Do **not** add `-r win-x64` on this multi-target MAUI project — restore can fail looking for Mono runtimes.

### Version bumps

Edit in [`publish-msix.ps1`](../publish-msix.ps1):

- `$displayVersion` — user-visible version (e.g. `1.0.1`)

The output folder name changes (e.g. `EnterprisePOS_1.0.0.1_Test`) — update the paths in this doc or use the path printed by `publish-msix.ps1`.

For package identity version, align [`Package.appxmanifest`](../Platforms/Windows/Package.appxmanifest) `Identity Version` or use MSBuild properties your pipeline defines.

---

## Manifest and display name

Windows package metadata: [`Platforms/Windows/Package.appxmanifest`](../Platforms/Windows/Package.appxmanifest)

- Display name: **Enterprise POS**
- Desktop capability: `runFullTrust` (unpackaged-style MAUI desktop)

---

## Troubleshooting

| Symptom | What to try |
|---------|-------------|
| App exits immediately on `dotnet run` or exe | Unset `COREHOST_TRACE`; use `.\run.ps1` |
| MSIX publish very slow | Normal on first Release MSIX; avoid `dotnet clean` unless needed |
| Publish OK but no `.msix` | Check `bin\Release\...\win-x64\AppPackages` |
| **0x800B010A** / publisher not verified | Run `.\install-msix.ps1` (or trust `certs\EnterprisePOS.cer` in **Trusted Root**) |
| No Desktop shortcut | Run `.\create-desktop-shortcut.ps1` |
| Many `XC0022` / `XC0025` warnings | XamlC binding warnings; build still succeeds. Optional: add `x:DataType` on templates later |
| `mspdbcmf.exe` not found | Symbol package skipped when `GenerateAppxSymbolPackage=false` |

Log a full publish to a file:

```powershell
.\publish-msix.ps1 *>&1 | Tee-Object -FilePath publish_msix.log
```

---

## What not to change for a safe dev build

- Do **not** set `WindowsPackageType=MSIX` in the `.csproj` for everyday development — use `publish-msix.ps1` or manual `-p:WindowsPackageType=MSIX` on publish only.
- Do **not** commit `certs/*.pfx` (already in `.gitignore`).
- Re-enable excluded XAML (Dashboard, Themes) only after WinUI stability is confirmed — see [ROADMAP.md](ROADMAP.md).

---

## Related files

| File | Purpose |
|------|---------|
| [`watch.ps1`](../watch.ps1) | Live dev loop (`dotnet watch`) |
| [`run.ps1`](../run.ps1) | Debug build + launch (one-off) |
| [`publish-msix.ps1`](../publish-msix.ps1) | Signed MSIX publish |
| [`install-msix.ps1`](../install-msix.ps1) | Trust cert + install MSIX + desktop shortcut |
| [`create-desktop-shortcut.ps1`](../create-desktop-shortcut.ps1) | Desktop shortcut only |
| [`EnterprisePOS.csproj`](../EnterprisePOS.csproj) | `WindowsPackageType=None` default |
| [`Package.appxmanifest`](../Platforms/Windows/Package.appxmanifest) | MSIX identity and capabilities |
