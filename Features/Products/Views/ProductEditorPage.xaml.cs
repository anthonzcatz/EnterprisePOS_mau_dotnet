using Microsoft.Maui.Controls;
using EnterprisePOS.Features.Products.ViewModels;
using EnterprisePOS.Helpers;

namespace EnterprisePOS.Features.Products.Views
{
	public partial class ProductEditorPage : ContentPage
	{
		private bool _isInitialized = false;

		public ProductEditorPage(ProductEditorViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
			_isInitialized = true;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (!_isInitialized) return;
			UpdateLayout(Width, Height);
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			if (!_isInitialized) return;
			base.OnSizeAllocated(width, height);
			if (width <= 0 || height <= 0) return;
			UpdateLayout(width, height);
		}

		private void UpdateLayout(double width, double height)
		{
			var isWide = width >= LayoutBreakpoints.WideLayoutMin;
			
			var wideLayout = this.FindByName<Grid>("WideLayout");
			var narrowLayout = this.FindByName<Grid>("NarrowLayout");
			
			if (wideLayout != null && narrowLayout != null)
			{
				wideLayout.IsVisible = isWide;
				narrowLayout.IsVisible = !isWide;
			}
		}

		private async void OnBackClicked(object? sender, EventArgs e)
		{
			await Shell.Current.GoToAsync("..");
		}
	}
}
