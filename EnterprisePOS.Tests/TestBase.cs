using Microsoft.EntityFrameworkCore;
using EnterprisePOS.Core.Data.Local;
using EnterprisePOS.Core.Data.Interfaces;

namespace EnterprisePOS.Tests;

public abstract class TestBase : IDisposable
{
    protected ILocalDbContext Context { get; private set; } = null!;
    protected LocalDbContext DbContext { get; private set; } = null!;

    protected TestBase()
    {
        SetupContext();
    }

    private void SetupContext()
    {
        var options = new DbContextOptionsBuilder<LocalDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        DbContext = new LocalDbContext(options);
        Context = DbContext;
        
        // Ensure database is created
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        DbContext?.Database?.EnsureDeleted();
        DbContext?.Dispose();
        GC.SuppressFinalize(this);
    }
}
