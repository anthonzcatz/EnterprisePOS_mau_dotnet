using EnterprisePOS.Interfaces;
using EnterprisePOS.Helpers;

namespace EnterprisePOS.Repositories;

/// <summary>Placeholder until API/MySQL repositories are wired.</summary>
public abstract class InMemoryRepository<T> : IRepository<T> where T : class
{
	protected readonly List<T> _data = new();

	public virtual Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			return Task.FromResult<IReadOnlyList<T>>(_data.AsReadOnly());
		}
		catch (Exception ex)
		{
			// TODO: Add logging here
			return Task.FromException<IReadOnlyList<T>>(ex);
		}
	}

	public virtual Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		try
		{
			return Task.FromResult<T?>(null);
		}
		catch (Exception ex)
		{
			// TODO: Add logging here
			return Task.FromException<T?>(ex);
		}
	}

	public virtual Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
	{
		try
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			_data.Add(entity);
			return Task.FromResult(entity);
		}
		catch (Exception ex)
		{
			// TODO: Add logging here
			return Task.FromException<T>(ex);
		}
	}

	public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
	{
		try
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			return Task.CompletedTask;
		}
		catch (Exception ex)
		{
			// TODO: Add logging here
			return Task.FromException(ex);
		}
	}

	public virtual Task DeleteAsync(string id, CancellationToken cancellationToken = default)
	{
		try
		{
			if (string.IsNullOrEmpty(id))
				throw new ArgumentException("Id cannot be null or empty", nameof(id));

			return Task.CompletedTask;
		}
		catch (Exception ex)
		{
			// TODO: Add logging here
			return Task.FromException(ex);
		}
	}

	public virtual Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
	{
		try
		{
			if (string.IsNullOrEmpty(id))
				throw new ArgumentException("Id cannot be null or empty", nameof(id));

			return Task.FromResult(false);
		}
		catch (Exception ex)
		{
			// TODO: Add logging here
			return Task.FromException<bool>(ex);
		}
	}
}
