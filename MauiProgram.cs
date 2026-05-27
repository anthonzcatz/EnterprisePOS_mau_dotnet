using Microsoft.Extensions.Logging;
using EnterprisePOS.Interfaces;
using EnterprisePOS.Services;
using EnterprisePOS.ViewModels;
using EnterprisePOS.Views;

namespace EnterprisePOS;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Services (minimal for stable Windows POS startup)
		builder.Services.AddSingleton<ThemeService>();
		builder.Services.AddSingleton<IPosService, MockPosService>();

		// ViewModels + Views (POS only)
		builder.Services.AddTransient<POSViewModel>();
		builder.Services.AddTransient<POSPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
