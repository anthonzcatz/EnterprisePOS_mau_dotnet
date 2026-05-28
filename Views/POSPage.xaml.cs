using System.Linq;
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
