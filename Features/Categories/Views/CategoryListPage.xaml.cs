using EnterprisePOS.Features.Categories.ViewModels;

namespace EnterprisePOS.Features.Categories.Views
{
    public partial class CategoryListPage : ContentPage
    {
        public CategoryListPage(CategoryListViewModel viewModel)
        {
            try
            {
                InitializeComponent();
                BindingContext = viewModel;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CategoryListPage] Constructor error: {ex}");
            }
        }
    }
}
