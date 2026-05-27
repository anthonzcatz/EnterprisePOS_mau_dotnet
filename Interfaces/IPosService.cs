using EnterprisePOS.Models;

namespace EnterprisePOS.Interfaces;

public interface IPosService
{
	Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken = default);
}
