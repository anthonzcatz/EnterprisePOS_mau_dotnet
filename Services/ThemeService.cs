namespace EnterprisePOS.Services;

public enum EnterpriseTheme
{
	Light,
	Dark
}

public sealed class ThemeService
{
	private const string PreferenceKey = "enterprise_theme";

	public EnterpriseTheme GetSavedTheme()
	{
		var saved = Microsoft.Maui.Storage.Preferences.Default.Get(PreferenceKey, string.Empty);
		if (string.IsNullOrWhiteSpace(saved))
		{
			return Application.Current?.RequestedTheme == AppTheme.Dark
				? EnterpriseTheme.Dark
				: EnterpriseTheme.Light;
		}

		return saved.Equals(nameof(EnterpriseTheme.Dark), StringComparison.OrdinalIgnoreCase)
			? EnterpriseTheme.Dark
			: EnterpriseTheme.Light;
	}

	public EnterpriseTheme GetCurrentTheme()
	{
		return Application.Current?.UserAppTheme == AppTheme.Dark
			? EnterpriseTheme.Dark
			: EnterpriseTheme.Light;
	}

	public void ApplySavedTheme()
	{
		ApplyTheme(GetSavedTheme(), persistPreference: false);
	}

	public void ApplyTheme(EnterpriseTheme theme, bool persistPreference)
	{
		if (Application.Current is null)
		{
			return;
		}

		Application.Current.UserAppTheme = theme == EnterpriseTheme.Dark
			? AppTheme.Dark
			: AppTheme.Light;

		if (persistPreference)
		{
			Microsoft.Maui.Storage.Preferences.Default.Set(PreferenceKey, theme.ToString());
		}
	}

	public void ApplyTheme(EnterpriseTheme theme)
	{
		ApplyTheme(theme, persistPreference: false);
	}
}
