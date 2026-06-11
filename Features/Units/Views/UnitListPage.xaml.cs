using EnterprisePOS.Features.Units.ViewModels;

namespace EnterprisePOS.Features.Units.Views
{
    public partial class UnitListPage : ContentPage
    {
        public UnitListPage(UnitListViewModel viewModel)
        {
            try
            {
                InitializeComponent();
                BindingContext = viewModel;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UnitListPage] Constructor error: {ex}");
            }
        }
    }
}
