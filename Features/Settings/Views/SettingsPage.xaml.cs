using EnterprisePOS.Helpers;
using EnterprisePOS.Features.Settings.ViewModels;

namespace EnterprisePOS.Features.Settings.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
		BindingContext = ServiceHelper.GetRequiredService<SettingsViewModel>();
	}
}
