using EnterprisePOS.Helpers;
using EnterprisePOS.Features.Dashboard.ViewModels;

namespace EnterprisePOS.Features.Dashboard.Views;

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
