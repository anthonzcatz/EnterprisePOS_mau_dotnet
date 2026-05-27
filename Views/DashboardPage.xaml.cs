using EnterprisePOS.Helpers;
using EnterprisePOS.ViewModels;

namespace EnterprisePOS.Views;

public partial class DashboardPage : ContentPage
{
	private readonly DashboardViewModel viewModel;

	public DashboardPage()
	{
		InitializeComponent();
		BindingContext = viewModel = ServiceHelper.GetRequiredService<DashboardViewModel>();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await viewModel.InitializeAsync();
	}
}
