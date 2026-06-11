using EnterprisePOS.Core.Data.Local;
using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Core.Data.Repositories;

public class UnitRepository
{
    private readonly LocalDbContext _context;

    public UnitRepository(LocalDbContext context)
    {
        _context = context;
    }

    public async Task<Unit?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Units
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Unit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Units
            .Where(u => u.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Unit> AddAsync(Unit unit, CancellationToken cancellationToken = default)
    {
        _context.Units.Add(unit);
        await _context.SaveChangesAsync(cancellationToken);
        return unit;
    }

    public async Task<Unit> UpdateAsync(Unit unit, CancellationToken cancellationToken = default)
    {
        _context.Units.Update(unit);
        await _context.SaveChangesAsync(cancellationToken);
        return unit;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var unit = await GetByIdAsync(id, cancellationToken);
        if (unit != null)
        {
            unit.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
