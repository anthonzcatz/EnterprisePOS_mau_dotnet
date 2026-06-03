using EnterprisePOS.Core.Data.Local;
using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Core.Data.Repositories;

public class ProductStockRepository
{
    private readonly LocalDbContext _context;

    public ProductStockRepository(LocalDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductStock>> GetAllWithProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductStocks
            .Include(ps => ps.Product)
                .ThenInclude(p => p.Category)
            .Include(ps => ps.Product)
                .ThenInclude(p => p.Unit)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductStock?> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductStocks
            .Include(ps => ps.Product)
            .FirstOrDefaultAsync(ps => ps.ProductId == productId, cancellationToken);
    }

    public async Task<ProductStock> UpdateStockAsync(int productId, decimal quantityChange, CancellationToken cancellationToken = default)
    {
        var stock = await GetByProductIdAsync(productId, cancellationToken);
        if (stock == null)
        {
            stock = new ProductStock
            {
                ProductId = productId,
                BaseQuantity = quantityChange,
                DisplayQuantity = quantityChange
            };
            _context.ProductStocks.Add(stock);
        }
        else
        {
            stock.BaseQuantity += quantityChange;
            stock.DisplayQuantity += quantityChange;
            _context.ProductStocks.Update(stock);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return stock;
    }
}
