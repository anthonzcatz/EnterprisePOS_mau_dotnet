namespace EnterprisePOS;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());
		window.Created += (_, _) =>
		{
			try
			{
				Helpers.ServiceHelper.GetRequiredService<Services.ThemeService>().ApplySavedTheme();
			}
			catch
			{
				// Theme applies on first Settings visit if services not ready yet.
			}
		};
		return window;
	}
}
