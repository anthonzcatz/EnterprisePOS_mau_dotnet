namespace EnterprisePOS.Interfaces;

public interface IRepository<T> where T : class
{
	Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
