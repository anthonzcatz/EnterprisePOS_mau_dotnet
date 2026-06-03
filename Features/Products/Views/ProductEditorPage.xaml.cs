using Microsoft.Maui.Controls;
using EnterprisePOS.Features.Products.ViewModels;

namespace EnterprisePOS.Features.Products.Views
{
	public partial class ProductEditorPage : ContentPage
	{
		private readonly ProductEditorViewModel _viewModel;

		public ProductEditorPage(ProductEditorViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = _viewModel = viewModel;
		}
	}
}
