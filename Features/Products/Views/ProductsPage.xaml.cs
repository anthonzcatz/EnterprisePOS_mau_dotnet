using EnterprisePOS.Features.Products.ViewModels;

namespace EnterprisePOS.Features.Products.Views
{
	public partial class ProductsPage : ContentPage
	{
		private ProductsViewModel? _viewModel;

		public ProductsPage(ProductsViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
			_viewModel = viewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			Console.WriteLine($"[ProductsPage] OnAppearing called");
			if (_viewModel != null)
			{
				Console.WriteLine($"[ProductsPage] Calling RefreshCommand");
				_viewModel.RefreshCommand.Execute(null);
			}
		}
	}
}
