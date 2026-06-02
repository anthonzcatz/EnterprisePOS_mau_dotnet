# Database Integration Guide

## Overview

This guide explains how to integrate the MySQL database with the EnterprisePOS MAUI application.

## Database Schema

The complete database schema is available in `docs/DATABASE_SCHEMA.sql`.

### Key Tables

**Users & Authentication:**
- `users` - User accounts
- `roles` - User roles
- `permissions` - System permissions
- `role_permissions` - Role-permission mapping

**Organization:**
- `branches` - Business branches
- `pos_terminals` - POS terminals per branch

**Products:**
- `products` - Product catalog
- `product_categories` - Product categories
- `product_variants` - Product variants (size, color, etc.)
- `product_addons` - Product add-ons (extra shots, toppings, etc.)
- `units` - Measurement units
- `product_stocks` - Inventory levels per branch

**Sales:**
- `sales` - Sales transactions
- `sale_items` - Items in a sale
- `sale_item_addons` - Add-ons for sale items
- `sale_payments` - Payment records
- `payment_methods` - Payment method types

**Inventory:**
- `stock_movements` - Stock movement history
- `suppliers` - Supplier information
- `purchase_orders` - Purchase orders
- `purchase_order_items` - Items in purchase orders

**Financial:**
- `receivables` - Customer receivables
- `receivable_payments` - Receivable payments
- `cash_drawers` - Cash drawer management
- `cash_drawer_sessions` - Cash drawer sessions
- `expenses` - Expense tracking

**Compliance:**
- `bir_settings` - BIR compliance settings (Philippines)
- `receipt_series` - Receipt number series
- `z_readings` - End-of-day Z-readings

**System:**
- `system_settings` - Application settings
- `number_sequences` - Auto-number sequences
- `audit_logs` - Audit trail
- `error_logs` - Error logging
- `notifications` - User notifications

## Integration Options

### Option 1: Direct MySQL Connection (Recommended for Windows)

**Pros:**
- Direct database access
- No additional API layer needed
- Faster development for Windows-only deployment

**Cons:**
- Requires MySQL server
- Not suitable for mobile (Android/iOS)
- Security concerns with direct database access

**Implementation:**
```csharp
// Install NuGet packages:
// - MySql.Data
// - Dapper (optional for easier queries)

using MySql.Data.MySqlClient;

public class DatabaseService
{
    private readonly string _connectionString;
    
    public DatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<List<Product>> GetProductsAsync()
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        const string sql = @"
            SELECT p.*, c.name as category_name, u.abbreviation as unit
            FROM products p
            LEFT JOIN product_categories c ON p.category_id = c.id
            LEFT JOIN units u ON p.unit_id = u.id
            WHERE p.is_active = TRUE";
            
        return (await connection.QueryAsync<Product>(sql)).ToList();
    }
}
```

### Option 2: ASP.NET Core Web API (Recommended for Cross-Platform)

**Pros:**
- Works on all platforms (Windows, Android, iOS)
- Centralized business logic
- Better security (no direct database access)
- Easier to scale
- Offline capability with caching

**Cons:**
- Requires API server
- More complex architecture
- Network dependency

**Implementation:**

**API Side:**
```csharp
// ASP.NET Core Web API Project
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;
    
    public ProductsController(IProductRepository repository)
    {
        _repository = repository;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _repository.GetAllAsync();
        return Ok(products);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }
}
```

**MAUI Side:**
```csharp
// Install NuGet packages:
// - Microsoft.Extensions.Http
// - Refit (optional for typed HTTP clients)

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    
    public ApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["Api:BaseUrl"];
    }
    
    public async Task<List<ProductDto>> GetProductsAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/products");
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<List<ProductDto>>();
    }
}
```

### Option 3: SQLite for Offline + Sync (Recommended for Mobile POS)

**Pros:**
- Works offline
- Fast local queries
- Sync when online
- Best for mobile POS terminals

**Cons:**
- Requires sync logic
- More complex data management
- Potential sync conflicts

**Implementation:**
```csharp
// Install NuGet packages:
// - Microsoft.EntityFrameworkCore.Sqlite
// - Microsoft.EntityFrameworkCore.Tools

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "enterprisepos.db");
        options.UseSqlite($"Data Source={dbPath}");
    }
}

public class SyncService
{
    private readonly AppDbContext _localContext;
    private readonly ApiService _apiService;
    
    public async Task SyncProductsAsync()
    {
        var remoteProducts = await _apiService.GetProductsAsync();
        var localProducts = await _localContext.Products.ToListAsync();
        
        // Sync logic: add new, update changed, remove deleted
        foreach (var remote in remoteProducts)
        {
            var local = localProducts.FirstOrDefault(p => p.Id == remote.Id);
            if (local == null)
            {
                _localContext.Products.Add(remote);
            }
            else if (local.UpdatedAt < remote.UpdatedAt)
            {
                _localContext.Entry(local).CurrentValues.SetValues(remote);
            }
        }
        
        await _localContext.SaveChangesAsync();
    }
}
```

## Recommended Architecture

For EnterprisePOS, use **Option 2 (ASP.NET Core Web API)** with **Option 3 (SQLite for offline)**:

```
MAUI App (Windows/Android/iOS)
    ↓ (HTTP)
ASP.NET Core Web API
    ↓ (ADO.NET/Entity Framework)
MySQL Database
    ↓ (Sync)
SQLite (Local Cache for Offline)
```

## Configuration

### appsettings.json (API)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=enterprisepos;User=root;Password=your_password;SslMode=None"
  },
  "Jwt": {
    "SecretKey": "your-secret-key",
    "ExpiryMinutes": 60
  }
}
```

### appsettings.json (MAUI)
```json
{
  "Api": {
    "BaseUrl": "https://api.yourdomain.com",
    "TimeoutSeconds": 30
  },
  "Offline": {
    "EnableLocalCache": true,
    "SyncIntervalMinutes": 5
  }
}
```

## Repository Pattern Implementation

### Base Repository
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;
    
    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    
    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
```

### Product Repository
```csharp
public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetByCategoryAsync(int categoryId);
    Task<IReadOnlyList<Product>> SearchAsync(string searchTerm);
}

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }
    
    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .ToListAsync();
    }
    
    public async Task<IReadOnlyList<Product>> SearchAsync(string searchTerm)
    {
        return await _dbSet
            .Where(p => p.Name.Contains(searchTerm) || p.Code.Contains(searchTerm))
            .Where(p => p.IsActive)
            .ToListAsync();
    }
}
```

## Migration Strategy

### Using Entity Framework Migrations

```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef

# Add initial migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update
```

### Manual SQL Execution

```bash
# Execute the schema file
mysql -u root -p enterprisepos < docs/DATABASE_SCHEMA.sql
```

## Security Considerations

1. **Never store connection strings in code** - Use configuration files or environment variables
2. **Use parameterized queries** - Prevent SQL injection
3. **Implement authentication/authorization** - JWT tokens for API
4. **Encrypt sensitive data** - Passwords, credit card info
5. **Use HTTPS** - Secure API communication
6. **Implement rate limiting** - Prevent abuse
7. **Audit logging** - Track all data changes

## Performance Optimization

1. **Use connection pooling** - Reuse database connections
2. **Implement caching** - Redis or in-memory cache
3. **Use pagination** - For large datasets
4. **Add database indexes** - On frequently queried columns
5. **Use stored procedures** - For complex queries
6. **Implement read replicas** - For reporting queries

## Testing

### Unit Tests
```csharp
[Test]
public async Task GetProducts_ReturnsActiveProducts()
{
    // Arrange
    var mockContext = new Mock<AppDbContext>();
    var repository = new ProductRepository(mockContext.Object);
    
    // Act
    var products = await repository.GetAllAsync();
    
    // Assert
    Assert.That(products.All(p => p.IsActive));
}
```

### Integration Tests
```csharp
[Test]
public async Task CreateProduct_SavesToDatabase()
{
    // Arrange
    using var context = new AppDbContext(_options);
    var repository = new ProductRepository(context);
    var product = new Product { Name = "Test Product", Price = 100 };
    
    // Act
    var result = await repository.AddAsync(product);
    
    // Assert
    Assert.That(result.Id, Is.GreaterThan(0));
}
```

## Next Steps

1. Choose integration option (recommended: API + SQLite for offline)
2. Set up MySQL database and run schema
3. Create ASP.NET Core Web API project
4. Implement repositories and services
5. Add authentication/authorization
6. Integrate API with MAUI app
7. Implement offline sync
8. Add error handling and logging
9. Test on all platforms
10. Deploy and monitor
