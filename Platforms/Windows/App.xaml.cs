using System;
using System.IO;
using System.Text;
using Microsoft.Maui.Storage;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EnterprisePOS.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		this.InitializeComponent();

		// Capture UI-thread (XAML/binding) exceptions that bypass AppDomain handlers.
		this.UnhandledException += OnWinUiUnhandledException;
	}

	private void OnWinUiUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
	{
		try
		{
			var logPath = Path.Combine(FileSystem.Current.AppDataDirectory, "startup-errors.log");
			var directory = Path.GetDirectoryName(logPath);
			if (!string.IsNullOrWhiteSpace(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var builder = new StringBuilder()
				.AppendLine($"[{DateTimeOffset.Now:O}] WinUI UI-thread unhandled exception")
				.AppendLine($"Message: {e.Message}")
				.AppendLine(e.Exception?.ToString() ?? "(no exception object)")
				.AppendLine(new string('-', 80));

			File.AppendAllText(logPath, builder.ToString());
		}
		catch
		{
			// Ignore logging failures so they do not mask the original error.
		}

		// Prevent the process from terminating so we can keep the app alive for diagnosis.
		e.Handled = true;
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

