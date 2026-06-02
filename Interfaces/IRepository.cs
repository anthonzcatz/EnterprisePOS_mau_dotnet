namespace EnterprisePOS.Interfaces;

public interface IRepository<T> where T : class
{
	Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
	Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
	Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
	Task DeleteAsync(string id, CancellationToken cancellationToken = default);
	Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
}
