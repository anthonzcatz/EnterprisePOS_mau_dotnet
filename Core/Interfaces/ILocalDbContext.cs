using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Core.Data.Interfaces;

public interface ILocalDbContext
{
    DbSet<Product> Products { get; }
    DbSet<ProductCategory> ProductCategories { get; }
    DbSet<Unit> Units { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<ProductStock> ProductStocks { get; }
    DbSet<Sale> Sales { get; }
    DbSet<SaleItem> SaleItems { get; }
    DbSet<Branch> Branches { get; }
    DbSet<PosTerminal> PosTerminals { get; }
    DbSet<Customer> Customers { get; }
    DbSet<SalePayment> SalePayments { get; }
    DbSet<PaymentMethod> PaymentMethods { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
