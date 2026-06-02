using EnterprisePOS.Interfaces;
using EnterprisePOS.Features.POS.Models;
using EnterprisePOS.Repositories;

namespace EnterprisePOS.Features.POS.Repositories;

public class ProductRepository : InMemoryRepository<Product>
{
	public override Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		var product = _data.FirstOrDefault(p => p.Name == id);
		return Task.FromResult(product);
	}

	public override Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
	{
		var exists = _data.Any(p => p.Name == id);
		return Task.FromResult(exists);
	}
}
