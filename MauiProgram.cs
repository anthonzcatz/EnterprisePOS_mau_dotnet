using LiveChartsCore.SkiaSharpView.Maui;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls.Hosting;
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
using EnterprisePOS.Features.Customers.ViewModels;
using EnterprisePOS.Features.Sales.Views;
using EnterprisePOS.Features.Sales.ViewModels;
using EnterprisePOS.Features.Reports.Views;
using EnterprisePOS.Features.UserManagement.Views;
using EnterprisePOS.Features.UserManagement.ViewModels;
using EnterprisePOS.Features.Products.Views;
using EnterprisePOS.Features.Products.ViewModels;
using EnterprisePOS.Features.Inventory.Views;
using EnterprisePOS.Features.Inventory.ViewModels;
using EnterprisePOS.Core.Data.Local;
using EnterprisePOS.Core.Data.Repositories;
using EnterprisePOS.Core.Services;
using EnterprisePOS.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
				.UseSkiaSharp()
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				})
				.ConfigureMauiHandlers(handlers =>
				{
#if WINDOWS
					handlers.AddHandler<Microsoft.Maui.Controls.Entry, EnterprisePOS.Platforms.Windows.EntryHandlerCustom>();
					handlers.AddHandler<Microsoft.Maui.Controls.Picker, EnterprisePOS.Platforms.Windows.PickerHandlerCustom>();
#endif
				});

		// Database Services
		builder.Services.AddDbContext<LocalDbContext>(options =>
			options.UseSqlite("Data Source=local.db"));

		// Core Services
		builder.Services.AddSingleton<IImageService, ImageService>();

		// Repositories
		builder.Services.AddScoped<ProductRepository>();
		builder.Services.AddScoped<ProductCategoryRepository>();
		builder.Services.AddScoped<ProductStockRepository>();
		builder.Services.AddScoped<SaleRepository>();
		builder.Services.AddScoped<CustomerRepository>();
		builder.Services.AddScoped<UserRepository>();

		// Services
		builder.Services.AddSingleton<ThemeService>();
		builder.Services.AddSingleton<IPosService, MockPosService>();
		builder.Services.AddSingleton<ILoggingService, LoggingService>();
		builder.Services.AddSingleton<IDashboardService, MockDashboardService>();
		builder.Services.AddSingleton<AppShellViewModel>();

		// ViewModels + Views
		builder.Services.AddTransient<POSViewModel>();
		builder.Services.AddTransient<POSPage>();
		builder.Services.AddTransient<DashboardViewModel>();
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<DashboardHomePage>();
		builder.Services.AddTransient<SettingsViewModel>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<CustomersViewModel>();
		builder.Services.AddTransient<CustomersPage>();
		builder.Services.AddTransient<SalesViewModel>();
		builder.Services.AddTransient<SalesPage>();
		builder.Services.AddTransient<ReportsPage>();
		builder.Services.AddTransient<ProductsViewModel>();
		builder.Services.AddTransient<ProductEditorViewModel>();
		builder.Services.AddTransient<ProductsPage>();
		builder.Services.AddTransient<ProductEditorPage>();
		builder.Services.AddTransient<InventoryViewModel>();
		builder.Services.AddTransient<InventoryPage>();
		builder.Services.AddTransient<EnterprisePOS.Features.UserManagement.ViewModels.UserManagementViewModel>();
		builder.Services.AddTransient<EnterprisePOS.Features.UserManagement.Views.UserManagementPage>();
		builder.Services.AddTransient<EnterprisePOS.Features.UserManagement.ViewModels.CreateUserViewModel>();
		builder.Services.AddTransient<EnterprisePOS.Features.UserManagement.Views.CreateUserPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
