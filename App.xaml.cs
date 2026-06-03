#if WINDOWS
using WinUIWindow = Microsoft.UI.Xaml.Window;
using Windows.Graphics;
#endif

using EnterprisePOS.Core.Data.Local;

namespace EnterprisePOS;

public partial class App : Application
{
	private static Microsoft.Maui.Controls.Window? mainWindow;

	public App(LocalDbContext dbContext)
	{
		InitializeComponent();
		_ = DatabaseSeeder.SeedDatabaseAsync(dbContext);
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// Use AppShell for navigation
		var appShell = new AppShell();

		mainWindow = new Window(appShell)
		{
			Title = "EnterprisePOS",
			Width = 1400,
			Height = 900
		};

		mainWindow.Created += (_, _) =>
		{
#if WINDOWS
			ForceActivateWindow();
#endif
		};

		return mainWindow;
	}

#if WINDOWS
	public static void ForceActivateWindow()
	{
		if (mainWindow?.Handler?.PlatformView is not WinUIWindow platformWindow)
		{
			return;
		}

		try
		{
			platformWindow.AppWindow.Move(new Windows.Graphics.PointInt32(100, 100));
			platformWindow.AppWindow.Resize(new SizeInt32(1400, 900));
			platformWindow.Activate();
		}
		catch
		{
			// Ignore activation failures.
		}
	}
#endif
}
