# [OPEN] Debug Session: windows-app-not-launching

## Summary
- Symptom: `dotnet run` on Windows completes or starts, but no application window appears.
- Goal: Determine whether the problem is caused by startup failure, window creation failure, process lock/state, or environment/tooling issues.

## Hypotheses
1. The Windows target is failing during app startup before the main window is shown.
2. A previously running `EnterprisePOS.exe` process is interfering with the new launch.
3. A XAML/source-generated startup issue is preventing the app window from being created.
4. The app launches but is immediately hidden, minimized, or blocked by an exception on first render.
5. `dotnet run` is targeting/building successfully, but the runtime entry path is misconfigured for Windows launch.

## Evidence Plan
- Capture `dotnet run` output for Windows target.
- Check for active `EnterprisePOS` processes before and after launch.
- Inspect startup files (`App.xaml.cs`, `AppShell.xaml.cs`, Windows platform entry) without modifying logic.
- If needed, add minimal instrumentation only after narrowing the observation points.

## Status
- Session opened.
