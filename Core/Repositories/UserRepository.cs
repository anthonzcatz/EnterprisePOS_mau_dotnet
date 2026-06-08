using EnterprisePOS.Core.Data.Local;
using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Core.Data.Repositories;

public class UserRepository
{
    private readonly LocalDbContext _context;

    public UserRepository(LocalDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);
        var normalizedUsername = username.Trim().ToLower();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == normalizedUsername, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);
        var normalizedEmail = email.Trim().ToLower();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);
        return await _context.Users
            .Where(u => u.IsActive && !u.IsDeleted)
            .OrderBy(u => u.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);
        return await _context.Users
            .Where(u => u.IsActive && !u.IsDeleted && u.Role == role)
            .OrderBy(u => u.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByBranchAsync(string branch, CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);
        return await _context.Users
            .Where(u => u.IsActive && !u.IsDeleted && u.Branch == branch)
            .OrderBy(u => u.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);
        user.Username = user.Username.Trim().ToLower();
        user.Email = user.Email.Trim().ToLower();
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);
        user.Username = user.Username.Trim().ToLower();
        user.Email = user.Email.Trim().ToLower();
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(id, cancellationToken);
        if (user != null)
        {
            user.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);
        var normalizedUsername = username.Trim().ToLower();
        return await _context.Users
            .AnyAsync(u => u.Username == normalizedUsername && !u.IsDeleted, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        await EnsureSchemaAsync(cancellationToken);
        var normalizedEmail = email.Trim().ToLower();
        return await _context.Users
            .AnyAsync(u => u.Email == normalizedEmail && !u.IsDeleted, cancellationToken);
    }

    private async Task EnsureSchemaAsync(CancellationToken cancellationToken)
    {
        await _context.Database.EnsureCreatedAsync(cancellationToken);
        await DatabaseSeeder.EnsureUserManagementSchemaAsync(_context);
    }
}
