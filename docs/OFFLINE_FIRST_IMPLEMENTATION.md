# Offline-First Implementation Guide

## Overview

This document provides a comprehensive guide for implementing the offline-first architecture in EnterprisePOS. The implementation follows industry best practices for scalability, performance, and security.

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Technology Stack](#technology-stack)
3. [Project Structure](#project-structure)
4. [Phase 1: Local SQLite Database](#phase-1-local-sqlite-database)
5. [Phase 2: Sync Queue Infrastructure](#phase-2-sync-queue-infrastructure)
6. [Phase 3: Connection Detection](#phase-3-connection-detection)
7. [Phase 4: Sync Service](#phase-4-sync-service)
8. [Phase 5: Dual Repository Pattern](#phase-5-dual-repository-pattern)
9. [Phase 6: Conflict Resolution](#phase-6-conflict-resolution)
10. [Phase 7: Security Implementation](#phase-7-security-implementation)
11. [Phase 8: Performance Optimization](#phase-8-performance-optimization)
12. [Testing Strategy](#testing-strategy)
13. [Deployment Guide](#deployment-guide)

## Architecture Overview

### Component Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                     WPF Application Layer                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │   ViewModels │  │   Services   │  │  Repositories │          │
│  └──────────────┘  └──────────────┘  └──────────────┘          │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Data Access Layer                             │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │ SQLite EF    │  │ Sync Queue   │  │ Connection   │          │
│  │ Core Context │  │   Service    │  │   Service    │          │
│  └──────────────┘  └──────────────┘  └──────────────┘          │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    HTTP Client Layer                            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │ HttpClient   │  │ JWT Handler  │  │ Retry Policy │          │
│  └──────────────┘  └──────────────┘  └──────────────┘          │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│              ASP.NET Core Web API (Remote)                      │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │ Controllers  │  │   Services   │  │ MySQL EF     │          │
│  └──────────────┘  └──────────────┘  └──────────────┘          │
└─────────────────────────────────────────────────────────────────┘
```

## Technology Stack

### Required NuGet Packages

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
<PackageReference Include="System.Text.Json" Version="8.0.0" />
<PackageReference Include="Polly" Version="8.0.0" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.0" />
```

### Database Technologies
- **SQLite 3.x** - Local offline database
- **MySQL 8.0+** - Remote production database
- **Entity Framework Core 8.0** - ORM for both databases

### Security Libraries
- **System.Security.Cryptography** - Encryption
- **Microsoft.IdentityModel.Tokens** - JWT handling
- **Windows.Security.Cryptography** - Windows DPAPI (for key storage)

## Project Structure

```
EnterprisePOS/
├── Core/
│   ├── Data/
│   │   ├── Local/
│   │   │   ├── ILocalDbContext.cs
│   │   │   ├── LocalDbContext.cs
│   │   │   └── Migrations/
│   │   ├── Remote/
│   │   │   ├── IRemoteDbContext.cs
│   │   │   └── RemoteDbContext.cs
│   │   ├── Models/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── Product.cs
│   │   │   ├── Customer.cs
│   │   │   ├── Sale.cs
│   │   │   └── ...
│   │   └── Interfaces/
│   │       ├── IRepository.cs
│   │       ├── ILocalRepository.cs
│   │       └── IRemoteRepository.cs
│   ├── Sync/
│   │   ├── Models/
│   │   │   ├── SyncOperation.cs
│   │   │   ├── SyncStatus.cs
│   │   │   └── SyncResult.cs
│   │   ├── Interfaces/
│   │   │   ├── ISyncQueue.cs
│   │   │   ├── ISyncService.cs
│   │   │   └── IConflictResolver.cs
│   │   └── Services/
│   │       ├── SyncQueue.cs
│   │       ├── SyncService.cs
│   │       └── ConflictResolver.cs
│   ├── Network/
│   │   ├── Interfaces/
│   │   │   └── IConnectionService.cs
│   │   └── Services/
│   │       └── ConnectionService.cs
│   └── Security/
│       ├── Interfaces/
│       │   ├── IEncryptionService.cs
│       │   └── IAuthService.cs
│       └── Services/
│           ├── EncryptionService.cs
│           └── AuthService.cs
├── Features/
│   └── POS/
│       ├── Repositories/
│       │   ├── ProductRepository.cs
│       │   ├── CustomerRepository.cs
│       │   └── SaleRepository.cs
│       └── Services/
│           └── PosService.cs
└── Infrastructure/
    ├── Http/
    │   ├── Interfaces/
    │   │   └── IApiClient.cs
    │   └── Services/
    │       └── ApiClient.cs
    └── Configuration/
        └── AppSettings.cs
```

## Phase 1: Local SQLite Database

### 1.1 Base Entity

```csharp
namespace EnterprisePOS.Core.Data.Models;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Sync fields
    public string? ServerId { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public bool NeedsSync { get; set; } = false;
}
```

### 1.2 Local DbContext Interface

```csharp
namespace EnterprisePOS.Core.Data.Local;

public interface ILocalDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Sale> Sales { get; }
    DbSet<SaleItem> SaleItems { get; }
    DbSet<SyncOperation> SyncOperations { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### 1.3 Local DbContext Implementation

```csharp
namespace EnterprisePOS.Core.Data.Local;

public class LocalDbContext : DbContext, ILocalDbContext
{
    private readonly string _dbPath;
    
    public LocalDbContext()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _dbPath = Path.Combine(appData, "EnterprisePOS", "local.db");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        
        // Performance optimizations
        optionsBuilder.EnableSensitiveDataLogging(false);
        optionsBuilder.EnableDetailedErrors(false);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure indexes for performance
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Code)
            .IsUnique();
            
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name);
            
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Code)
            .IsUnique();
            
        modelBuilder.Entity<Sale>()
            .HasIndex(s => s.SaleDate);
            
        modelBuilder.Entity<SyncOperation>()
            .HasIndex(s => s.Status);
            
        modelBuilder.Entity<SyncOperation>()
            .HasIndex(s => s.CreatedAt);
    }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<SyncOperation> SyncOperations { get; set; }
}
```

### 1.4 Database Initialization Service

```csharp
namespace EnterprisePOS.Core.Data.Local;

public class DatabaseInitializationService
{
    private readonly ILocalDbContext _context;
    
    public DatabaseInitializationService(ILocalDbContext context)
    {
        _context = context;
    }
    
    public async Task InitializeAsync()
    {
        // Ensure database directory exists
        var dbPath = ((LocalDbContext)_context).GetDatabasePath();
        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // Create database if not exists
        await ((DbContext)_context).Database.EnsureCreatedAsync();
        
        // Run migrations if any
        await ((DbContext)_context).Database.MigrateAsync();
    }
}
```

### 1.5 Dependency Injection Setup

```csharp
// In App.xaml.cs or Program.cs
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;
    
    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        
        // Register DbContext
        services.AddDbContext<LocalDbContext>(options =>
            options.UseSqlite("Data Source=local.db"));
            
        services.AddScoped<ILocalDbContext, LocalDbContext>();
        services.AddScoped<DatabaseInitializationService>();
        
        _serviceProvider = services.BuildServiceProvider();
        
        // Initialize database
        var dbInit = _serviceProvider.GetRequiredService<DatabaseInitializationService>();
        dbInit.InitializeAsync().Wait();
        
        base.OnStartup(e);
    }
}
```

## Phase 2: Sync Queue Infrastructure

### 2.1 Sync Operation Model

```csharp
namespace EnterprisePOS.Core.Sync.Models;

public class SyncOperation
{
    public long Id { get; set; }
    public string EntityType { get; set; } = string.Empty; // "Product", "Customer", "Sale"
    public string Operation { get; set; } = string.Empty; // "Create", "Update", "Delete"
    public string EntityData { get; set; } = string.Empty; // JSON serialized entity
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SyncedAt { get; set; }
    public SyncStatus Status { get; set; } = SyncStatus.Pending;
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; } = 0;
    public string? ServerEntityId { get; set; } // ID from server after sync
}

public enum SyncStatus
{
    Pending,
    Syncing,
    Synced,
    Failed,
    Cancelled
}
```

### 2.2 Sync Queue Interface

```csharp
namespace EnterprisePOS.Core.Sync.Interfaces;

public interface ISyncQueue
{
    Task<long> EnqueueAsync(SyncOperation operation);
    Task<SyncOperation?> DequeueAsync();
    Task<IEnumerable<SyncOperation>> GetPendingOperationsAsync(int batchSize = 100);
    Task UpdateStatusAsync(long operationId, SyncStatus status, string? errorMessage = null);
    Task MarkAsSyncedAsync(long operationId, string serverEntityId);
    Task ClearSyncedOperationsAsync(DateTime beforeDate);
    Task<int> GetPendingCountAsync();
}
```

### 2.3 Sync Queue Implementation

```csharp
namespace EnterprisePOS.Core.Sync.Services;

public class SyncQueue : ISyncQueue
{
    private readonly ILocalDbContext _context;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    public SyncQueue(ILocalDbContext context)
    {
        _context = context;
    }
    
    public async Task<long> EnqueueAsync(SyncOperation operation)
    {
        await _semaphore.WaitAsync();
        try
        {
            _context.SyncOperations.Add(operation);
            await _context.SaveChangesAsync();
            return operation.Id;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task<SyncOperation?> DequeueAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var operation = await _context.SyncOperations
                .Where(s => s.Status == SyncStatus.Pending)
                .OrderBy(s => s.CreatedAt)
                .FirstOrDefaultAsync();
                
            if (operation != null)
            {
                operation.Status = SyncStatus.Syncing;
                await _context.SaveChangesAsync();
            }
            
            return operation;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task<IEnumerable<SyncOperation>> GetPendingOperationsAsync(int batchSize = 100)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _context.SyncOperations
                .Where(s => s.Status == SyncStatus.Pending)
                .OrderBy(s => s.CreatedAt)
                .Take(batchSize)
                .ToListAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task UpdateStatusAsync(long operationId, SyncStatus status, string? errorMessage = null)
    {
        await _semaphore.WaitAsync();
        try
        {
            var operation = await _context.SyncOperations.FindAsync(operationId);
            if (operation != null)
            {
                operation.Status = status;
                operation.ErrorMessage = errorMessage;
                if (status == SyncStatus.Failed)
                {
                    operation.RetryCount++;
                }
                await _context.SaveChangesAsync();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task MarkAsSyncedAsync(long operationId, string serverEntityId)
    {
        await _semaphore.WaitAsync();
        try
        {
            var operation = await _context.SyncOperations.FindAsync(operationId);
            if (operation != null)
            {
                operation.Status = SyncStatus.Synced;
                operation.SyncedAt = DateTime.UtcNow;
                operation.ServerEntityId = serverEntityId;
                await _context.SaveChangesAsync();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task ClearSyncedOperationsAsync(DateTime beforeDate)
    {
        await _semaphore.WaitAsync();
        try
        {
            var operations = await _context.SyncOperations
                .Where(s => s.Status == SyncStatus.Synced && s.SyncedAt < beforeDate)
                .ToListAsync();
                
            _context.SyncOperations.RemoveRange(operations);
            await _context.SaveChangesAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task<int> GetPendingCountAsync()
    {
        return await _context.SyncOperations
            .CountAsync(s => s.Status == SyncStatus.Pending);
    }
}
```

## Phase 3: Connection Detection

### 3.1 Connection Service Interface

```csharp
namespace EnterprisePOS.Core.Network.Interfaces;

public interface IConnectionService
{
    bool IsOnline { get; }
    ConnectionStatus Status { get; }
    event EventHandler<ConnectionStatusChangedEventArgs> ConnectionChanged;
    Task<bool> CheckConnectionAsync();
    Task StartMonitoringAsync();
    Task StopMonitoringAsync();
}

public class ConnectionStatusChangedEventArgs : EventArgs
{
    public ConnectionStatus OldStatus { get; set; }
    public ConnectionStatus NewStatus { get; set; }
}

public enum ConnectionStatus
{
    Unknown,
    Online,
    Offline,
    Unstable
}
```

### 3.2 Connection Service Implementation

```csharp
namespace EnterprisePOS.Core.Network.Services;

public class ConnectionService : IConnectionService
{
    private readonly HttpClient _httpClient;
    private readonly Timer _monitorTimer;
    private readonly string _apiBaseUrl;
    private ConnectionStatus _status = ConnectionStatus.Unknown;
    private bool _isMonitoring = false;
    
    public bool IsOnline => _status == ConnectionStatus.Online;
    public ConnectionStatus Status => _status;
    
    public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionChanged = null!;
    
    public ConnectionService(IConfiguration configuration)
    {
        _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://api.example.com";
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };
        _monitorTimer = new Timer(CheckConnectionCallback, null, Timeout.Infinite, Timeout.Infinite);
    }
    
    public async Task<bool> CheckConnectionAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/health");
            var isOnline = response.IsSuccessStatusCode;
            
            var newStatus = isOnline ? ConnectionStatus.Online : ConnectionStatus.Offline;
            
            if (_status != newStatus)
            {
                var oldStatus = _status;
                _status = newStatus;
                ConnectionChanged?.Invoke(this, new ConnectionStatusChangedEventArgs
                {
                    OldStatus = oldStatus,
                    NewStatus = newStatus
                });
            }
            
            return isOnline;
        }
        catch
        {
            if (_status != ConnectionStatus.Offline)
            {
                var oldStatus = _status;
                _status = ConnectionStatus.Offline;
                ConnectionChanged?.Invoke(this, new ConnectionStatusChangedEventArgs
                {
                    OldStatus = oldStatus,
                    NewStatus = _status
                });
            }
            return false;
        }
    }
    
    public async Task StartMonitoringAsync()
    {
        if (_isMonitoring) return;
        
        _isMonitoring = true;
        _monitorTimer.Change(0, 30000); // Check every 30 seconds
        await CheckConnectionAsync();
    }
    
    public async Task StopMonitoringAsync()
    {
        _isMonitoring = false;
        await _monitorTimer.DisposeAsync();
    }
    
    private async void CheckConnectionCallback(object? state)
    {
        if (_isMonitoring)
        {
            await CheckConnectionAsync();
        }
    }
}
```

## Phase 4: Sync Service

### 4.1 Sync Service Interface

```csharp
namespace EnterprisePOS.Core.Sync.Interfaces;

public interface ISyncService
{
    Task<SyncResult> SyncAsync();
    Task<SyncResult> UploadPendingOperationsAsync();
    Task<SyncResult> DownloadChangesAsync(DateTime? lastSyncTime);
    bool IsSyncing { get; }
    event EventHandler<SyncProgressEventArgs> SyncProgress;
}

public class SyncResult
{
    public bool Success { get; set; }
    public int UploadedCount { get; set; }
    public int DownloadedCount { get; set; }
    public int FailedCount { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan Duration { get; set; }
}

public class SyncProgressEventArgs : EventArgs
{
    public string CurrentOperation { get; set; } = string.Empty;
    public int TotalOperations { get; set; }
    public int CompletedOperations { get; set; }
    public double ProgressPercentage => TotalOperations > 0 
        ? (double)CompletedOperations / TotalOperations * 100 
        : 0;
}
```

### 4.2 Sync Service Implementation

```csharp
namespace EnterprisePOS.Core.Sync.Services;

public class SyncService : ISyncService
{
    private readonly ISyncQueue _syncQueue;
    private readonly IConnectionService _connectionService;
    private readonly IApiClient _apiClient;
    private readonly IConflictResolver _conflictResolver;
    private readonly ILocalDbContext _context;
    private bool _isSyncing = false;
    
    public bool IsSyncing => _isSyncing;
    public event EventHandler<SyncProgressEventArgs> SyncProgress = null!;
    
    public SyncService(
        ISyncQueue syncQueue,
        IConnectionService connectionService,
        IApiClient apiClient,
        IConflictResolver conflictResolver,
        ILocalDbContext context)
    {
        _syncQueue = syncQueue;
        _connectionService = connectionService;
        _apiClient = apiClient;
        _conflictResolver = conflictResolver;
        _context = context;
    }
    
    public async Task<SyncResult> SyncAsync()
    {
        if (_isSyncing)
        {
            return new SyncResult
            {
                Success = false,
                ErrorMessage = "Sync already in progress"
            };
        }
        
        _isSyncing = true;
        var stopwatch = Stopwatch.StartNew();
        var result = new SyncResult();
        
        try
        {
            if (!_connectionService.IsOnline)
            {
                return new SyncResult
                {
                    Success = false,
                    ErrorMessage = "No internet connection"
                };
            }
            
            // Upload pending operations
            var uploadResult = await UploadPendingOperationsAsync();
            result.UploadedCount = uploadResult.UploadedCount;
            result.FailedCount += uploadResult.FailedCount;
            
            if (!uploadResult.Success)
            {
                result.ErrorMessage = uploadResult.ErrorMessage;
                return result;
            }
            
            // Download changes
            var lastSyncTime = await GetLastSyncTimeAsync();
            var downloadResult = await DownloadChangesAsync(lastSyncTime);
            result.DownloadedCount = downloadResult.DownloadedCount;
            result.FailedCount += downloadResult.FailedCount;
            
            if (!downloadResult.Success)
            {
                result.ErrorMessage = downloadResult.ErrorMessage;
                return result;
            }
            
            // Update last sync time
            await UpdateLastSyncTimeAsync();
            
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
            _isSyncing = false;
        }
        
        return result;
    }
    
    public async Task<SyncResult> UploadPendingOperationsAsync()
    {
        var result = new SyncResult();
        var operations = await _syncQueue.GetPendingOperationsAsync(100);
        
        SyncProgress?.Invoke(this, new SyncProgressEventArgs
        {
            CurrentOperation = "Uploading changes",
            TotalOperations = operations.Count(),
            CompletedOperations = 0
        });
        
        foreach (var operation in operations)
        {
            try
            {
                await _syncQueue.UpdateStatusAsync(operation.Id, SyncStatus.Syncing);
                
                var success = await ProcessOperationAsync(operation);
                
                if (success)
                {
                    await _syncQueue.MarkAsSyncedAsync(operation.Id, operation.ServerEntityId ?? "");
                    result.UploadedCount++;
                }
                else
                {
                    await _syncQueue.UpdateStatusAsync(operation.Id, SyncStatus.Failed, "Processing failed");
                    result.FailedCount++;
                }
            }
            catch (Exception ex)
            {
                await _syncQueue.UpdateStatusAsync(operation.Id, SyncStatus.Failed, ex.Message);
                result.FailedCount++;
            }
            
            SyncProgress?.Invoke(this, new SyncProgressEventArgs
            {
                CurrentOperation = "Uploading changes",
                TotalOperations = operations.Count(),
                CompletedOperations = result.UploadedCount + result.FailedCount
            });
        }
        
        result.Success = result.FailedCount == 0;
        return result;
    }
    
    public async Task<SyncResult> DownloadChangesAsync(DateTime? lastSyncTime)
    {
        var result = new SyncResult();
        
        try
        {
            var changes = await _apiClient.GetChangesAsync(lastSyncTime);
            
            SyncProgress?.Invoke(this, new SyncProgressEventArgs
            {
                CurrentOperation = "Downloading changes",
                TotalOperations = changes.Count(),
                CompletedOperations = 0
            });
            
            foreach (var change in changes)
            {
                await ApplyChangeAsync(change);
                result.DownloadedCount++;
                
                SyncProgress?.Invoke(this, new SyncProgressEventArgs
                {
                    CurrentOperation = "Downloading changes",
                    TotalOperations = changes.Count(),
                    CompletedOperations = result.DownloadedCount
                });
            }
            
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        
        return result;
    }
    
    private async Task<bool> ProcessOperationAsync(SyncOperation operation)
    {
        return operation.Operation switch
        {
            "Create" => await ProcessCreateAsync(operation),
            "Update" => await ProcessUpdateAsync(operation),
            "Delete" => await ProcessDeleteAsync(operation),
            _ => false
        };
    }
    
    private async Task<bool> ProcessCreateAsync(SyncOperation operation)
    {
        var response = await _apiClient.CreateEntityAsync(operation.EntityType, operation.EntityData);
        if (response.Success)
        {
            operation.ServerEntityId = response.EntityId;
            return true;
        }
        return false;
    }
    
    private async Task<bool> ProcessUpdateAsync(SyncOperation operation)
    {
        var response = await _apiClient.UpdateEntityAsync(operation.EntityType, operation.EntityData);
        return response.Success;
    }
    
    private async Task<bool> ProcessDeleteAsync(SyncOperation operation)
    {
        var response = await _apiClient.DeleteEntityAsync(operation.EntityType, operation.EntityData);
        return response.Success;
    }
    
    private async Task ApplyChangeAsync(ServerChange change)
    {
        // Check for conflicts
        var localEntity = await GetLocalEntityAsync(change.EntityType, change.EntityId);
        
        if (localEntity != null)
        {
            var conflict = await _conflictResolver.ResolveConflictAsync(localEntity, change);
            await ApplyResolutionAsync(conflict);
        }
        else
        {
            // No conflict, apply change directly
            await ApplyChangeDirectlyAsync(change);
        }
    }
    
    private async Task<object?> GetLocalEntityAsync(string entityType, string entityId)
    {
        return entityType switch
        {
            "Product" => await _context.Products.FindAsync(int.Parse(entityId)),
            "Customer" => await _context.Customers.FindAsync(int.Parse(entityId)),
            "Sale" => await _context.Sales.FindAsync(int.Parse(entityId)),
            _ => null
        };
    }
    
    private async Task ApplyChangeDirectlyAsync(ServerChange change)
    {
        var entity = JsonSerializer.Deserialize(change.EntityData, GetEntityType(change.EntityType));
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
    
    private async Task ApplyResolutionAsync(ConflictResolution resolution)
    {
        // Apply the resolved entity
        await _context.AddAsync(resolution.ResolvedEntity);
        await _context.SaveChangesAsync();
    }
    
    private Type GetEntityType(string entityType)
    {
        return entityType switch
        {
            "Product" => typeof(Product),
            "Customer" => typeof(Customer),
            "Sale" => typeof(Sale),
            _ => throw new ArgumentException($"Unknown entity type: {entityType}")
        };
    }
    
    private async Task<DateTime?> GetLastSyncTimeAsync()
    {
        // Store last sync time in local settings or database
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == "LastSyncTime");
        return setting != null ? DateTime.Parse(setting.Value) : null;
    }
    
    private async Task UpdateLastSyncTimeAsync()
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == "LastSyncTime");
            
        if (setting != null)
        {
            setting.Value = DateTime.UtcNow.ToString("O");
        }
        else
        {
            _context.SystemSettings.Add(new SystemSetting
            {
                Key = "LastSyncTime",
                Value = DateTime.UtcNow.ToString("O")
            });
        }
        
        await _context.SaveChangesAsync();
    }
}
```

## Phase 5: Dual Repository Pattern

### 5.1 Repository Interfaces

```csharp
namespace EnterprisePOS.Core.Data.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public interface ILocalRepository<T> : IRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetPendingSyncAsync();
}

public interface IRemoteRepository<T> : IRepository<T> where T : BaseEntity
{
    Task<T?> GetByServerIdAsync(string serverId);
}
```

### 5.2 Dual Repository Implementation

```csharp
namespace EnterprisePOS.Features.POS.Repositories;

public class ProductRepository : IRepository<Product>
{
    private readonly IConnectionService _connectionService;
    private readonly ILocalRepository<Product> _localRepo;
    private readonly IRemoteRepository<Product> _remoteRepo;
    private readonly ISyncQueue _syncQueue;
    
    public ProductRepository(
        IConnectionService connectionService,
        ILocalRepository<Product> localRepo,
        IRemoteRepository<Product> remoteRepo,
        ISyncQueue syncQueue)
    {
        _connectionService = connectionService;
        _localRepo = localRepo;
        _remoteRepo = remoteRepo;
        _syncQueue = syncQueue;
    }
    
    public async Task<Product?> GetByIdAsync(int id)
    {
        if (_connectionService.IsOnline)
        {
            // Try remote first, fallback to local
            var product = await _remoteRepo.GetByIdAsync(id);
            if (product != null)
            {
                // Update local cache
                await _localRepo.UpdateAsync(product);
                return product;
            }
        }
        return await _localRepo.GetByIdAsync(id);
    }
    
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        if (_connectionService.IsOnline)
        {
            var products = await _remoteRepo.GetAllAsync();
            // Update local cache
            foreach (var product in products)
            {
                var existing = await _localRepo.GetByIdAsync(product.Id);
                if (existing != null)
                {
                    await _localRepo.UpdateAsync(product);
                }
                else
                {
                    await _localRepo.AddAsync(product);
                }
            }
            return products;
        }
        return await _localRepo.GetAllAsync();
    }
    
    public async Task<Product> AddAsync(Product product)
    {
        // Always add to local first
        var localProduct = await _localRepo.AddAsync(product);
        localProduct.NeedsSync = true;
        
        if (_connectionService.IsOnline)
        {
            // Sync to server immediately
            var serverProduct = await _remoteRepo.AddAsync(product);
            localProduct.ServerId = serverProduct.Id.ToString();
            localProduct.NeedsSync = false;
            await _localRepo.UpdateAsync(localProduct);
        }
        else
        {
            // Queue for sync
            await _syncQueue.EnqueueAsync(new SyncOperation
            {
                EntityType = "Product",
                Operation = "Create",
                EntityData = JsonSerializer.Serialize(product)
            });
        }
        
        return localProduct;
    }
    
    public async Task<Product> UpdateAsync(Product product)
    {
        var localProduct = await _localRepo.UpdateAsync(product);
        localProduct.NeedsSync = true;
        
        if (_connectionService.IsOnline)
        {
            await _remoteRepo.UpdateAsync(product);
            localProduct.NeedsSync = false;
            await _localRepo.UpdateAsync(localProduct);
        }
        else
        {
            await _syncQueue.EnqueueAsync(new SyncOperation
            {
                EntityType = "Product",
                Operation = "Update",
                EntityData = JsonSerializer.Serialize(product)
            });
        }
        
        return localProduct;
    }
    
    public async Task DeleteAsync(int id)
    {
        await _localRepo.DeleteAsync(id);
        
        if (_connectionService.IsOnline)
        {
            await _remoteRepo.DeleteAsync(id);
        }
        else
        {
            await _syncQueue.EnqueueAsync(new SyncOperation
            {
                EntityType = "Product",
                Operation = "Delete",
                EntityData = JsonSerializer.Serialize(new { Id = id })
            });
        }
    }
}
```

## Phase 6: Conflict Resolution

### 6.1 Conflict Resolver Interface

```csharp
namespace EnterprisePOS.Core.Sync.Interfaces;

public interface IConflictResolver
{
    Task<ConflictResolution> ResolveConflictAsync(object localEntity, ServerChange serverChange);
}

public class ConflictResolution
{
    public object ResolvedEntity { get; set; } = null!;
    public ConflictResolutionStrategy Strategy { get; set; }
    public string? Reason { get; set; }
}

public enum ConflictResolutionStrategy
{
    ServerWins,
    ClientWins,
    LastWriteWins,
    ManualResolution,
    Merge
}

public class ServerChange
{
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string EntityData { get; set; } = string.Empty;
    public DateTime ModifiedAt { get; set; }
}
```

### 6.2 Conflict Resolver Implementation

```csharp
namespace EnterprisePOS.Core.Sync.Services;

public class ConflictResolver : IConflictResolver
{
    public async Task<ConflictResolution> ResolveConflictAsync(object localEntity, ServerChange serverChange)
    {
        var localModifiedAt = GetModifiedAt(localEntity);
        
        // Determine strategy based on entity type and timestamps
        var strategy = DetermineStrategy(localEntity, serverChange, localModifiedAt);
        
        var resolvedEntity = strategy switch
        {
            ConflictResolutionStrategy.ServerWins => JsonSerializer.Deserialize(
                serverChange.EntityData, 
                localEntity.GetType()),
            ConflictResolutionStrategy.ClientWins => localEntity,
            ConflictResolutionStrategy.LastWriteWins => localModifiedAt > serverChange.ModifiedAt 
                ? localEntity 
                : JsonSerializer.Deserialize(serverChange.EntityData, localEntity.GetType()),
            ConflictResolutionStrategy.Merge => await MergeEntitiesAsync(localEntity, serverChange),
            _ => localEntity
        };
        
        return new ConflictResolution
        {
            ResolvedEntity = resolvedEntity,
            Strategy = strategy,
            Reason = $"Resolved using {strategy} strategy"
        };
    }
    
    private ConflictResolutionStrategy DetermineStrategy(
        object localEntity, 
        ServerChange serverChange, 
        DateTime localModifiedAt)
    {
        // Critical data: Server wins
        if (IsCriticalData(serverChange.EntityType))
        {
            return ConflictResolutionStrategy.ServerWins;
        }
        
        // If timestamps are close (< 1 minute), use last-write-wins
        if (Math.Abs((localModifiedAt - serverChange.ModifiedAt).TotalMinutes) < 1)
        {
            return ConflictResolutionStrategy.LastWriteWins;
        }
        
        // Default: Last-write-wins
        return ConflictResolutionStrategy.LastWriteWins;
    }
    
    private bool IsCriticalData(string entityType)
    {
        // Define which entity types are critical
        var criticalTypes = new[] { "Sale", "Payment", "Inventory" };
        return criticalTypes.Contains(entityType);
    }
    
    private DateTime GetModifiedAt(object entity)
    {
        var property = entity.GetType().GetProperty("UpdatedAt");
        return property?.GetValue(entity) as DateTime? ?? DateTime.MinValue;
    }
    
    private async Task<object> MergeEntitiesAsync(object localEntity, ServerChange serverChange)
    {
        // Implement custom merge logic if needed
        // For now, default to server wins
        return JsonSerializer.Deserialize(serverChange.EntityData, localEntity.GetType())!;
    }
}
```

## Phase 7: Security Implementation

### 7.1 Encryption Service

```csharp
namespace EnterprisePOS.Core.Security.Interfaces;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
    byte[] EncryptBytes(byte[] plainBytes);
    byte[] DecryptBytes(byte[] cipherBytes);
}
```

### 7.2 Encryption Service Implementation

```csharp
namespace EnterprisePOS.Core.Security.Services;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _encryptionKey;
    private readonly byte[] _iv;
    
    public EncryptionService(IConfiguration configuration)
    {
        // In production, load key from secure storage
        var keyString = configuration["EncryptionKey"] ?? GenerateKey();
        _encryptionKey = Convert.FromBase64String(keyString);
        _iv = new byte[16]; // Use a fixed IV or generate per encryption
    }
    
    public string Encrypt(string plainText)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = EncryptBytes(plainBytes);
        return Convert.ToBase64String(cipherBytes);
    }
    
    public string Decrypt(string cipherText)
    {
        var cipherBytes = Convert.FromBase64String(cipherText);
        var plainBytes = DecryptBytes(cipherBytes);
        return Encoding.UTF8.GetString(plainBytes);
    }
    
    public byte[] EncryptBytes(byte[] plainBytes)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        
        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(plainBytes, 0, plainBytes.Length);
        }
        return ms.ToArray();
    }
    
    public byte[] DecryptBytes(byte[] cipherBytes)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        
        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
        {
            cs.Write(cipherBytes, 0, cipherBytes.Length);
        }
        return ms.ToArray();
    }
    
    private string GenerateKey()
    {
        using var aes = Aes.Create();
        aes.GenerateKey();
        return Convert.ToBase64String(aes.Key);
    }
}
```

### 7.3 Database Encryption

```csharp
// In LocalDbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Configure sensitive data encryption
    modelBuilder.Entity<Customer>()
        .Property(c => c.Phone)
        .HasConversion(
            v => _encryptionService.Encrypt(v),
            v => _encryptionService.Decrypt(v));
}
```

## Phase 8: Performance Optimization

### 8.1 Connection Pooling

```csharp
// In DbContext configuration
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlite($"Data Source={_dbPath}", options =>
    {
        options.CommandTimeout(30);
    });
    
    // Enable connection pooling
    SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
}
```

### 8.2 Batch Operations

```csharp
public async Task BulkInsertAsync(IEnumerable<Product> products)
{
    await _context.Products.AddRangeAsync(products);
    await _context.SaveChangesAsync();
}
```

### 8.3 Index Optimization

```csharp
// In OnModelCreating
modelBuilder.Entity<Product>()
    .HasIndex(p => new { p.Code, p.Name })
    .HasFilter("IsDeleted = 0");
```

### 8.4 Query Optimization

```csharp
// Use AsNoTracking for read-only queries
public async Task<IEnumerable<Product>> GetAllReadOnlyAsync()
{
    return await _context.Products
        .AsNoTracking()
        .Where(p => !p.IsDeleted)
        .ToListAsync();
}
```

## Testing Strategy

### 8.1 Unit Tests

```csharp
[TestClass]
public class SyncQueueTests
{
    [TestMethod]
    public async Task EnqueueAsync_ShouldAddOperation()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var queue = new SyncQueue(context);
        var operation = new SyncOperation
        {
            EntityType = "Product",
            Operation = "Create",
            EntityData = "{}"
        };
        
        // Act
        var id = await queue.EnqueueAsync(operation);
        
        // Assert
        Assert.IsTrue(id > 0);
    }
}
```

### 8.2 Integration Tests

```csharp
[TestClass]
public class SyncServiceTests
{
    [TestMethod]
    public async Task SyncAsync_ShouldSyncWhenOnline()
    {
        // Arrange
        var mockConnection = new MockConnectionService(true);
        var syncService = CreateSyncService(mockConnection);
        
        // Act
        var result = await syncService.SyncAsync();
        
        // Assert
        Assert.IsTrue(result.Success);
    }
}
```

## Deployment Guide

### 9.1 Configuration

```json
{
  "ApiBaseUrl": "https://api.example.com",
  "EncryptionKey": "base64-encoded-key",
  "SyncIntervalMinutes": 5,
  "BatchSize": 100,
  "MaxRetryCount": 3
}
```

### 9.2 Deployment Steps

1. **Build Application**
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained
   ```

2. **Configure Database**
   - SQLite will be created automatically
   - Ensure write permissions to AppData folder

3. **Test Offline Mode**
   - Disconnect network
   - Verify all features work
   - Reconnect and verify sync

4. **Monitor Sync**
   - Check sync queue
   - Verify conflict resolution
   - Monitor performance

## Best Practices

### Performance
- Use batch operations for bulk data
- Implement lazy loading for large datasets
- Cache reference data in memory
- Use indexes on frequently queried columns

### Security
- Encrypt sensitive data at rest
- Use HTTPS for all API calls
- Implement proper authentication
- Securely store encryption keys

### Scalability
- Implement pagination for large datasets
- Use background sync to avoid blocking UI
- Optimize sync batch sizes
- Monitor and tune performance

### Reliability
- Implement retry logic with exponential backoff
- Handle network failures gracefully
- Provide user feedback on sync status
- Implement proper error logging

## Troubleshooting

### Common Issues

1. **Sync Fails**
   - Check network connection
   - Verify API endpoint is accessible
   - Check sync queue for errors
   - Review server logs

2. **Database Locked**
   - Ensure single DbContext instance per operation
   - Use proper async/await patterns
   - Close connections properly

3. **Memory Issues**
   - Implement pagination
   - Use AsNoTracking for read queries
   - Dispose DbContext properly

4. **Slow Performance**
   - Check indexes
   - Optimize queries
   - Reduce batch size
   - Enable query caching

## Conclusion

This implementation provides a robust, scalable, and secure offline-first architecture for EnterprisePOS. The modular design allows for easy maintenance and future enhancements while ensuring data consistency across online and offline modes.
