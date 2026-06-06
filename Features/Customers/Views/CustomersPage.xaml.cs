using EnterprisePOS.Features.Customers.ViewModels;

namespace EnterprisePOS.Features.Customers.Views
{
	public partial class CustomersPage : ContentPage
	{
		private readonly CustomersViewModel _viewModel;

		public CustomersPage(CustomersViewModel viewModel)
		{
			_viewModel = viewModel;
			InitializeComponent();
			BindingContext = viewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			_viewModel.RefreshCommand.Execute(null);
		}
	}
}
