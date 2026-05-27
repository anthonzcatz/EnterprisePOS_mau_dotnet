namespace EnterprisePOS.Interfaces;

/// <summary>ASP.NET Core API client abstraction (JWT-ready).</summary>
public interface IApiService
{
	Task<T?> GetAsync<T>(string relativeUrl, CancellationToken cancellationToken = default);
	Task<T?> PostAsync<T>(string relativeUrl, object body, CancellationToken cancellationToken = default);
}
