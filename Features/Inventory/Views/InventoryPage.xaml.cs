using EnterprisePOS.Features.Inventory.ViewModels;

namespace EnterprisePOS.Features.Inventory.Views
{
	public partial class InventoryPage : ContentPage
	{
		private readonly InventoryViewModel _viewModel;

		public InventoryPage(InventoryViewModel viewModel)
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
