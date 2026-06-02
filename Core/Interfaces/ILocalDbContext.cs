namespace EnterprisePOS.Core.Data.Interfaces;

public interface ILocalDbContext
{
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
