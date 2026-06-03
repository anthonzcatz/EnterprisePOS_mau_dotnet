using EnterprisePOS.Features.Customers.ViewModels;

namespace EnterprisePOS.Features.Customers.Views
{
	public partial class CustomersPage : ContentPage
	{
		public CustomersPage(CustomersViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
