namespace EnterprisePOS.Interfaces;

public interface INavigationService
{
	Task GoToAsync(string route);
}
