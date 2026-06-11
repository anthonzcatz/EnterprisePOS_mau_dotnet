#if WINDOWS
using WinUIWindow = Microsoft.UI.Xaml.Window;
using Windows.Graphics;
#endif

using EnterprisePOS.Core.Data.Local;
using System.Text;

namespace EnterprisePOS;

public partial class App : Application
{
	private static Microsoft.Maui.Controls.Window? mainWindow;
	private static readonly string StartupLogPath = Path.Combine(FileSystem.Current.AppDataDirectory, "startup-errors.log");

	public App(LocalDbContext dbContext)
	{
		AttachExceptionHandlers();
		InitializeComponent();
	}

	private static async Task SeedDatabaseAsync(LocalDbContext dbContext)
	{
		try
		{
			await DatabaseSeeder.SeedDatabaseAsync(dbContext);
		}
		catch (Exception ex)
		{
			LogStartupException("Database seeding failed", ex);
		}
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		AppShell rootPage;
		try
		{
			rootPage = new AppShell();
		}
		catch (Exception ex)
		{
			LogStartupException("Failed to create AppShell", ex);
			throw;
		}

		mainWindow = new Window(rootPage)
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

	private static void AttachExceptionHandlers()
	{
		AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
		AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
		TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
		TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
	}

	private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		if (e.ExceptionObject is Exception ex)
		{
			LogStartupException("Unhandled exception", ex);
		}
	}

	private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
	{
		LogStartupException("Unobserved task exception", e.Exception);
	}

	private static void LogStartupException(string context, Exception exception)
	{
		try
		{
			var logDirectory = Path.GetDirectoryName(StartupLogPath);
			if (!string.IsNullOrWhiteSpace(logDirectory))
			{
				Directory.CreateDirectory(logDirectory);
			}

			var builder = new StringBuilder()
				.AppendLine($"[{DateTimeOffset.Now:O}] {context}")
				.AppendLine(exception.ToString())
				.AppendLine(new string('-', 80));

			File.AppendAllText(StartupLogPath, builder.ToString());
		}
		catch
		{
			// Ignore logging failures so they do not mask the original startup error.
		}
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
