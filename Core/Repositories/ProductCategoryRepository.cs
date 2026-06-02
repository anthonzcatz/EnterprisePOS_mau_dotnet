using EnterprisePOS.Core.Data.Local;
using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Core.Data.Repositories;

public class ProductCategoryRepository
{
    private readonly LocalDbContext _context;

    public ProductCategoryRepository(LocalDbContext context)
    {
        _context = context;
    }

    public async Task<ProductCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ProductCategories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductCategories
            .Include(c => c.ParentCategory)
            .Include(c => c.SubCategories)
            .Where(c => c.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductCategory>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductCategories
            .Where(c => c.ParentCategoryId == null && c.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductCategory> AddAsync(ProductCategory category, CancellationToken cancellationToken = default)
    {
        _context.ProductCategories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task<ProductCategory> UpdateAsync(ProductCategory category, CancellationToken cancellationToken = default)
    {
        _context.ProductCategories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await GetByIdAsync(id, cancellationToken);
        if (category != null)
        {
            category.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
