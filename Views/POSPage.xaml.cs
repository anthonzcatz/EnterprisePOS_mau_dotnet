using EnterprisePOS.Helpers;
using EnterprisePOS.ViewModels;

namespace EnterprisePOS.Views;

public partial class POSPage : ContentPage
{
	private readonly POSViewModel viewModel;

	public POSPage()
	{
		InitializeComponent();
		BindingContext = viewModel = ServiceHelper.GetRequiredService<POSViewModel>();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await viewModel.InitializeAsync();
		UpdateLayout(Width);
	}

	protected override void OnSizeAllocated(double width, double height)
	{
		base.OnSizeAllocated(width, height);
		UpdateLayout(width);
		viewModel.UpdateAvailableWidth(width);
	}

	private void UpdateLayout(double width)
	{
		var isWide = width >= LayoutBreakpoints.DesktopMin;
		WideLayout.IsVisible = isWide;
		NarrowLayout.IsVisible = !isWide;
	}
}
