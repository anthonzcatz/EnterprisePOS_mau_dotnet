using EnterprisePOS.Features.UserManagement.ViewModels;
using EnterprisePOS.Navigation;
using EnterprisePOS.Helpers;

namespace EnterprisePOS.Features.UserManagement.Views;

public partial class UserManagementPage : ContentPage
{
    private bool _isInitialized = false;

    public UserManagementPage(UserManagementViewModel viewModel)
    {
        try
        {
            InitializeComponent();
            BindingContext = viewModel;
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UserManagementPage] Constructor error: {ex}");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_isInitialized) return;
        
        try
        {
            UpdateLayout(Width, Height);
            if (BindingContext is UserManagementViewModel viewModel && !viewModel.IsBusy)
            {
                viewModel.RefreshCommand.Execute(null);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UserManagementPage] OnAppearing error: {ex.Message}");
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        if (!_isInitialized) return;

        try
        {
            // Guard against invalid dimensions
            if (width <= 0 || height <= 0) return;
            if (double.IsNaN(width) || double.IsNaN(height)) return;
            if (double.IsInfinity(width) || double.IsInfinity(height)) return;
            
            UpdateLayout(width, height);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UserManagementPage] Resize error: {ex.Message}");
        }
    }

    private void UpdateLayout(double width, double height)
    {
        try
        {
            var isWide = width >= LayoutBreakpoints.WideLayoutMin;
            
            // Use FindByName to safely get layouts
            var wideLayout = this.FindByName<Grid>("WideLayout");
            var narrowLayout = this.FindByName<Grid>("NarrowLayout");
            
            if (wideLayout != null && narrowLayout != null)
            {
                wideLayout.IsVisible = isWide;
                narrowLayout.IsVisible = !isWide;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UserManagementPage] UpdateLayout error: {ex.Message}");
        }
    }

    private async void OnAddUserClicked(object? sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync(Routes.UserMgmtCreate);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }
}
