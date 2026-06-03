using EnterprisePOS.Features.Inventory.ViewModels;

namespace EnterprisePOS.Features.Inventory.Views
{
	public partial class InventoryPage : ContentPage
	{
		public InventoryPage(InventoryViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
