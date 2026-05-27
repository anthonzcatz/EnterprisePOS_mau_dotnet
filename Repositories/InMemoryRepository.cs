using EnterprisePOS.Interfaces;

namespace EnterprisePOS.Repositories;

/// <summary>Placeholder until API/MySQL repositories are wired.</summary>
public abstract class InMemoryRepository<T> : IRepository<T> where T : class
{
	public virtual Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
		=> Task.FromResult<IReadOnlyList<T>>(Array.Empty<T>());

	public virtual Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
		=> Task.FromResult<T?>(null);
}
