using EnterprisePOS.Features.Sales.ViewModels;

namespace EnterprisePOS.Features.Sales.Views
{
	public partial class SalesPage : ContentPage
	{
		public SalesPage(SalesViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
