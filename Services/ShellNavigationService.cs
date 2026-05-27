using EnterprisePOS.Interfaces;

namespace EnterprisePOS.Services;

public sealed class ShellNavigationService : INavigationService
{
	public Task GoToAsync(string route)
	{
		return Shell.Current.GoToAsync(route);
	}
}
