using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace EnterprisePOS.Platforms.Windows;

public class EntryHandlerCustom : EntryHandler
{
    protected override void ConnectHandler(TextBox platformView)
    {
        base.ConnectHandler(platformView);
        platformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
        platformView.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
        platformView.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 209, 213, 219));
        platformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(1);
        platformView.CornerRadius = new Microsoft.UI.Xaml.CornerRadius(8);
        platformView.Padding = new Microsoft.UI.Xaml.Thickness(12, 8, 12, 8);
        
        // Maintain border on focus/hover
        platformView.GotFocus += (s, e) =>
        {
            platformView.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 209, 213, 219));
            platformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(1);
        };
        
        platformView.LostFocus += (s, e) =>
        {
            platformView.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 209, 213, 219));
            platformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(1);
        };
        
        platformView.PointerEntered += (s, e) =>
        {
            platformView.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 209, 213, 219));
            platformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(1);
        };
    }
}
