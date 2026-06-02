using EnterprisePOS.Core.Data.Local;
using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Core.Data.Repositories;

public class SaleRepository
{
    private readonly LocalDbContext _context;

    public SaleRepository(LocalDbContext context)
    {
        _context = context;
    }

    public async Task<Sale?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Branch)
            .Include(s => s.Terminal)
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
            .Include(s => s.SalePayments)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Branch)
            .Include(s => s.Terminal)
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
            .Include(s => s.SalePayments)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Branch)
            .Include(s => s.Terminal)
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
            .Include(s => s.SalePayments)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Branch)
            .Include(s => s.Terminal)
            .Include(s => s.Customer)
            .Include(s => s.SaleItems)
            .Include(s => s.SalePayments)
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Sale> AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale != null)
        {
            sale.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
