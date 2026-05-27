using Microsoft.Extensions.DependencyInjection;

namespace EnterprisePOS.Helpers;

public static class ServiceHelper
{
	public static T GetRequiredService<T>() where T : notnull
	{
		var services = IPlatformApplication.Current?.Services
			?? throw new InvalidOperationException("Application services are not initialized yet.");

		return services.GetRequiredService<T>();
	}

}
