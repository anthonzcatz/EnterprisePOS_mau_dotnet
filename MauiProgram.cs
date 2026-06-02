using Microsoft.Extensions.Logging;
using EnterprisePOS.Interfaces;
using EnterprisePOS.Services;
using EnterprisePOS.Features.POS.ViewModels;
using EnterprisePOS.Features.POS.Views;
using EnterprisePOS.Features.POS.Services;
using EnterprisePOS.Features.Dashboard.ViewModels;
using EnterprisePOS.Features.Dashboard.Views;
using EnterprisePOS.Features.Dashboard.Services;
using EnterprisePOS.Features.Settings.ViewModels;
using EnterprisePOS.Features.Settings.Views;
using EnterprisePOS.Features.Customers.Views;
using EnterprisePOS.Features.Sales.Views;
using EnterprisePOS.Features.Reports.Views;
using EnterprisePOS.Features.Users.Views;
using EnterprisePOS.Features.Products.Views;
using EnterprisePOS.Features.Inventory.Views;
using EnterprisePOS.Core.Data.Local;
using EnterprisePOS.Core.Services;
using Microsoft.EntityFrameworkCore;

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

		// Database Services
		builder.Services.AddDbContext<LocalDbContext>(options =>
			options.UseSqlite("Data Source=local.db"));

		// Core Services
		builder.Services.AddSingleton<IImageService, ImageService>();

		// Services
		builder.Services.AddSingleton<ThemeService>();
		builder.Services.AddSingleton<IPosService, PosServiceAdapter>();
		builder.Services.AddSingleton<ILoggingService, LoggingService>();
		builder.Services.AddSingleton<IDashboardService, MockDashboardService>();

		// ViewModels + Views
		builder.Services.AddTransient<POSViewModel>();
		builder.Services.AddTransient<POSPage>();
		builder.Services.AddTransient<DashboardViewModel>();
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<DashboardHomePage>();
		builder.Services.AddTransient<SettingsViewModel>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<CustomersPage>();
		builder.Services.AddTransient<SalesPage>();
		builder.Services.AddTransient<ReportsPage>();
		builder.Services.AddTransient<UsersPage>();
		builder.Services.AddTransient<ProductsPage>();
		builder.Services.AddTransient<InventoryPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
