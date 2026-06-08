using EnterprisePOS.Features.UserManagement.ViewModels;
using EnterprisePOS.Helpers;

namespace EnterprisePOS.Features.UserManagement.Views;

public partial class CreateUserPage : ContentPage
{
    private bool _isInitialized = false;

    public CreateUserPage(CreateUserViewModel viewModel)
    {
        try
        {
            InitializeComponent();
            BindingContext = viewModel;
            _isInitialized = true;
            System.Diagnostics.Debug.WriteLine("[CreateUserPage] Constructor completed, BindingContext set");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CreateUserPage] Constructor error: {ex}");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_isInitialized) return;
        
        try
        {
            UpdateLayout(Width, Height);
            
            // Debug: Check BindingContext and data
            if (BindingContext is CreateUserViewModel vm)
            {
                System.Diagnostics.Debug.WriteLine($"[CreateUserPage] OnAppearing - ViewModel found");
                System.Diagnostics.Debug.WriteLine($"[CreateUserPage] AvailableRoles count: {vm.AvailableRoles?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"[CreateUserPage] AvailableBranches count: {vm.AvailableBranches?.Count ?? 0}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[CreateUserPage] OnAppearing - BindingContext is null or not CreateUserViewModel");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CreateUserPage] OnAppearing error: {ex.Message}");
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        if (!_isInitialized) return;
        
        try
        {
            base.OnSizeAllocated(width, height);
            
            // Guard against invalid dimensions
            if (width <= 0 || height <= 0) return;
            if (double.IsNaN(width) || double.IsNaN(height)) return;
            if (double.IsInfinity(width) || double.IsInfinity(height)) return;
            
            UpdateLayout(width, height);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CreateUserPage] Resize error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[CreateUserPage] UpdateLayout error: {ex.Message}");
        }
    }

    
    private async void OnBackClicked(object? sender, EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }
}
