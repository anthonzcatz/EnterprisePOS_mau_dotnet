using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace EnterprisePOS.Platforms.Windows;

public class PickerHandlerCustom : PickerHandler
{
    protected override void ConnectHandler(ComboBox platformView)
    {
        base.ConnectHandler(platformView);
        
        // Apply basic styling without interfering with data binding
        platformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
        platformView.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
        platformView.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 209, 213, 219));
        platformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(1);
        platformView.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(8);
        platformView.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light;
        platformView.Padding = new Microsoft.UI.Xaml.Thickness(12, 8, 12, 8);
    }
}
