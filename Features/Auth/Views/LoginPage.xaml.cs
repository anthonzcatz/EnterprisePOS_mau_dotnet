using Microsoft.Maui.Controls;

namespace EnterprisePOS.Features.Auth.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            
            // Handle responsive layout based on screen size
            UpdateLayoutForScreenSize();
            
            // Listen for size changes
            SizeChanged += OnPageSizeChanged;
        }

        private void OnPageSizeChanged(object? sender, EventArgs e)
        {
            UpdateLayoutForScreenSize();
        }

        private const double WideLayoutThreshold = 900;

        private void UpdateLayoutForScreenSize()
        {
            // Switch purely on the actual available width so the layout is
            // reliable across phone, tablet (portrait/landscape) and desktop.
            // When Width is not yet measured (0), default to the mobile layout
            // so the page never renders blank.
            bool useWideLayout = Width >= WideLayoutThreshold;

            DesktopLayout.IsVisible = useWideLayout;
            MobileLayout.IsVisible = !useWideLayout;
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            // Default credentials for testing
            // Username: admin
            // Password: admin123
            
            // TODO: Implement actual authentication logic
            // For now, just navigate to the main app
            await Shell.Current.GoToAsync("//dashboard");
        }
    }
}
