using EnterprisePOS.Helpers;
using EnterprisePOS.ViewModels;

namespace EnterprisePOS.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
		BindingContext = ServiceHelper.GetRequiredService<SettingsViewModel>();
	}
}
