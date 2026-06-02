#if WINDOWS
using WinUIWindow = Microsoft.UI.Xaml.Window;
using Windows.Graphics;
#endif
using System.Diagnostics;
using EnterprisePOS.Core.Data.Local;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace EnterprisePOS;

public partial class App : Application
{
	private static Microsoft.Maui.Controls.Window? mainWindow;

	private static void LogToFile(string message)
	{
		try
		{
			var logPath = Path.Combine(AppContext.BaseDirectory, "app_startup.log");
			File.AppendAllText(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}{Environment.NewLine}");
		}
		catch { }
	}

	public App()
	{
		try
		{
			LogToFile("App constructor started");
			Debug.WriteLine("App constructor started");
			InitializeComponent();
			LogToFile("App constructor completed");
			Debug.WriteLine("App constructor completed");
		}
		catch (Exception ex)
		{
			LogToFile($"App constructor error: {ex.Message}");
			LogToFile($"Stack trace: {ex.StackTrace}");
			Debug.WriteLine($"App constructor error: {ex.Message}");
			Debug.WriteLine($"Stack trace: {ex.StackTrace}");
		}
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		try
		{
			LogToFile("CreateWindow started");
			Debug.WriteLine("=== CreateWindow started ===");

			// Use original DashboardPage with professional UI
			var dashboardPage = new Features.Dashboard.Views.DashboardPage();

			mainWindow = new Window(dashboardPage)
			{
				Title = "EnterprisePOS",
				Width = 1400,
				Height = 900
			};

			LogToFile("Window instance created");
			Debug.WriteLine("=== Window created successfully ===");

			mainWindow.Created += (_, _) =>
			{
				try
				{
					LogToFile("Window Created event fired");
					Debug.WriteLine("=== Window Created event fired ===");
					LogToFile("Window Created event completed");
					Debug.WriteLine("=== Window Created event completed ===");
				}
				catch (Exception ex)
				{
					LogToFile($"Window Created event error: {ex.Message}");
					LogToFile($"Stack trace: {ex.StackTrace}");
					Debug.WriteLine($"=== Window Created event error: {ex.Message} ===");
				}
			};

			mainWindow.Destroying += (_, _) =>
			{
				LogToFile("Window destroying");
				Debug.WriteLine("=== Window destroying ===");
			};

			LogToFile("Returning window from CreateWindow");
			Debug.WriteLine("=== Returning window ===");
			return mainWindow;
		}
		catch (Exception ex)
		{
			LogToFile($"CreateWindow error: {ex.Message}");
			LogToFile($"Stack trace: {ex.StackTrace}");
			Debug.WriteLine($"=== CreateWindow error: {ex.Message} ===");
			Debug.WriteLine($"=== Stack trace: {ex.StackTrace} ===");
			throw;
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
