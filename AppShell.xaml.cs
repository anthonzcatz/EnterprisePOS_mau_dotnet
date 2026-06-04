using EnterprisePOS.Features.Products.Views;
using EnterprisePOS.Navigation;

namespace EnterprisePOS;

public partial class AppShell : Shell
{
	private bool hasNavigatedToStartPage;

	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(Routes.ProductEditor, typeof(ProductEditorPage));
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (hasNavigatedToStartPage)
		{
			return;
		}

		hasNavigatedToStartPage = true;
		_ = GoToAsync("//dashboard/dashboard-main");
	}
}
