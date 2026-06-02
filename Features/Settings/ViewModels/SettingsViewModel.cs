using System.Windows.Input;
using EnterprisePOS.Helpers;
using EnterprisePOS.Services;

namespace EnterprisePOS.Features.Settings.ViewModels;

public sealed class SettingsViewModel : BaseViewModel
{
	private readonly ThemeService themeService;
	private bool isDark;
	private bool isApplyingTheme;

	public SettingsViewModel(ThemeService themeService)
	{
		this.themeService = themeService;
		isDark = themeService.GetSavedTheme() == EnterpriseTheme.Dark;
	}

	public bool IsDark
	{
		get => isDark;
		set
		{
			if (!SetProperty(ref isDark, value) || isApplyingTheme)
			{
				return;
			}

			isApplyingTheme = true;
			var next = value ? EnterpriseTheme.Dark : EnterpriseTheme.Light;
			themeService.ApplyTheme(next, persistPreference: true);
			isApplyingTheme = false;
		}
	}
}

