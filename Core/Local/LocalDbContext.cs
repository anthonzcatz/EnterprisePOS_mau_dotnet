using EnterprisePOS.Core.Data.Interfaces;
using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Core.Data.Local;

public class LocalDbContext : DbContext, ILocalDbContext
{
    private readonly string _databasePath;

    public LocalDbContext(string databasePath)
    {
        _databasePath = databasePath ?? throw new ArgumentNullException(nameof(databasePath));
    }

    public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options)
    {
        _databasePath = "local.db";
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
    public DbSet<Unit> Units { get; set; } = null!;
    public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
    public DbSet<ProductStock> ProductStocks { get; set; } = null!;
    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<SaleItem> SaleItems { get; set; } = null!;
    public DbSet<Branch> Branches { get; set; } = null!;
    public DbSet<PosTerminal> PosTerminals { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<SalePayment> SalePayments { get; set; } = null!;
    public DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite($"Data Source={_databasePath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Product configuration
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Code).IsUnique();
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // ProductCategory configuration
        modelBuilder.Entity<ProductCategory>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sale configuration
        modelBuilder.Entity<Sale>()
            .HasIndex(s => s.SaleNumber).IsUnique();
        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Branch)
            .WithMany(b => b.Sales)
            .HasForeignKey(s => s.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Terminal)
            .WithMany(t => t.Sales)
            .HasForeignKey(s => s.TerminalId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Sale>()
            .HasOne(s => s.Customer)
            .WithMany(c => c.Sales)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // SaleItem configuration
        modelBuilder.Entity<SaleItem>()
            .HasOne(si => si.Sale)
            .WithMany(s => s.SaleItems)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SaleItem>()
            .HasOne(si => si.Product)
            .WithMany()
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // SalePayment configuration
        modelBuilder.Entity<SalePayment>()
            .HasOne(sp => sp.Sale)
            .WithMany(s => s.SalePayments)
            .HasForeignKey(sp => sp.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SalePayment>()
            .HasOne(sp => sp.PaymentMethod)
            .WithMany(pm => pm.SalePayments)
            .HasForeignKey(sp => sp.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        // Terminal configuration
        modelBuilder.Entity<PosTerminal>()
            .HasOne(t => t.Branch)
            .WithMany(b => b.Terminals)
            .HasForeignKey(t => t.BranchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
