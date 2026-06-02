using EnterprisePOS.Interfaces;
using EnterprisePOS.Features.POS.Models;
using EnterprisePOS.Repositories;

namespace EnterprisePOS.Features.POS.Repositories;

public class CartRepository : InMemoryRepository<CartItem>
{
	public override Task<CartItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		var item = _data.FirstOrDefault(c => c.Name == id);
		return Task.FromResult(item);
	}

	public override Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
	{
		var exists = _data.Any(c => c.Name == id);
		return Task.FromResult(exists);
	}

	public override Task UpdateAsync(CartItem entity, CancellationToken cancellationToken = default)
	{
		var existing = _data.FirstOrDefault(c => c.Name == entity.Name);
		if (existing != null)
		{
			_data.Remove(existing);
			_data.Add(entity);
		}
		return Task.CompletedTask;
	}

	public override Task DeleteAsync(string id, CancellationToken cancellationToken = default)
	{
		var item = _data.FirstOrDefault(c => c.Name == id);
		if (item != null)
		{
			_data.Remove(item);
		}
		return Task.CompletedTask;
	}
}
