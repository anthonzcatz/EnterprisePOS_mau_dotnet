using EnterprisePOS.Features.Categories.ViewModels;

namespace EnterprisePOS.Features.Categories.Views
{
    public partial class CategoryEditorPage : ContentPage
    {
        public CategoryEditorPage(CategoryEditorViewModel viewModel)
        {
            try
            {
                InitializeComponent();
                BindingContext = viewModel;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CategoryEditorPage] Constructor error: {ex}");
            }
        }
    }
}
