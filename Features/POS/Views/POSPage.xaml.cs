using System.Linq;
using EnterprisePOS.Helpers;
using EnterprisePOS.Features.POS.ViewModels;

namespace EnterprisePOS.Features.POS.Views;

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
		try
		{
			await viewModel.InitializeAsync();
			UpdateLayout(Width, Height);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"POSPage OnAppearing error: {ex.Message}");
			await DisplayAlertAsync("Error", ex.Message, "OK");
		}
	}

	protected override void OnSizeAllocated(double width, double height)
	{
		base.OnSizeAllocated(width, height);
		UpdateLayout(width, height);
	}

	private void UpdateLayout(double width, double height)
	{
		var isLandscape = height > 0 && width / height >= 1.15;
		var isWide = width >= LayoutBreakpoints.DesktopMin ||
			(isLandscape && width >= LayoutBreakpoints.LargeTabletLandscapeMin);

		viewModel.UpdateAvailableWidth(width);

		WideLayout.IsVisible = isWide;
		NarrowLayout.IsVisible = !isWide;

		if (isWide)
		{
			viewModel.IsMobileSidebarOpen = false;
			viewModel.IsProfileMenuOpen = false;
		}
	}

	private void OnMobileCategoryScrollLeft(object? sender, TappedEventArgs e)
	{
		ScrollMobileCategory(-1);
	}

	private void OnMobileCategoryScrollRight(object? sender, TappedEventArgs e)
	{
		ScrollMobileCategory(1);
	}

	private void ScrollMobileCategory(int direction)
	{
		if (MobileCategoryStrip.ItemsSource is not System.Collections.IEnumerable items)
		{
			return;
		}

		var categories = items.Cast<object>().ToList();
		if (categories.Count == 0)
		{
			return;
		}

		var currentIndex = 0;
		if (BindingContext is POSViewModel vm)
		{
			currentIndex = Math.Max(0, vm.Categories.ToList().FindIndex(category => category.IsSelected));
		}

		var targetIndex = Math.Clamp(currentIndex + direction, 0, categories.Count - 1);
		MobileCategoryStrip.ScrollTo(targetIndex, position: ScrollToPosition.Center, animate: true);
	}
}
