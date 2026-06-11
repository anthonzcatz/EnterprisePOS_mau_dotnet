using EnterprisePOS.Features.Products.Views;
using EnterprisePOS.Features.Categories.Views;
using EnterprisePOS.Features.Units.Views;
using EnterprisePOS.Navigation;
using EnterprisePOS.Core.Data.Local;
using EnterprisePOS.Features.Auth.Views;

namespace EnterprisePOS;

public partial class AppShell : Shell
{
	private bool hasNavigatedToStartPage;

	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(Routes.ProductEditor, typeof(ProductEditorPage));
		Routing.RegisterRoute(Routes.Categories, typeof(CategoryListPage));
		Routing.RegisterRoute(Routes.CategoryEditor, typeof(CategoryEditorPage));
		Routing.RegisterRoute(Routes.Units, typeof(UnitListPage));
		Routing.RegisterRoute(Routes.UserMgmtCreate, typeof(EnterprisePOS.Features.UserManagement.Views.CreateUserPage));

		// Close flyout when navigation completes
		Navigated += (s, e) =>
		{
			FlyoutIsPresented = false;
		};

		// Close flyout when current item changes
		PropertyChanged += (s, e) =>
		{
			if (e.PropertyName == nameof(CurrentItem))
			{
				FlyoutIsPresented = false;
			}
		};
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (hasNavigatedToStartPage)
		{
			return;
		}

		hasNavigatedToStartPage = true;
		// Navigate to login page first
		_ = GoToAsync("//login");

		// Seed database after splash screen
		_ = SeedDatabaseAsync();
	}

	private async Task SeedDatabaseAsync()
	{
		try
		{
			var dbContext = Handler?.MauiContext?.Services.GetService<LocalDbContext>();
			if (dbContext != null)
			{
				await DatabaseSeeder.SeedDatabaseAsync(dbContext);
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[AppShell] Database seeding failed: {ex.Message}");
		}
	}
}
