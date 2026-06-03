using EnterprisePOS.Features.Products.ViewModels;

namespace EnterprisePOS.Features.Products.Views
{
	public partial class ProductsPage : ContentPage
	{
		public ProductsPage(ProductsViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
