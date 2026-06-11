using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Core.Data.Local;

public static class DatabaseSeeder
{
    public static async Task SeedDatabaseAsync(LocalDbContext context)
    {
        try
        {
            // Create database if it doesn't exist
            await context.Database.EnsureCreatedAsync();
            await EnsureUserManagementSchemaAsync(context);
            await EnsureUnitsSchemaAsync(context);
            await EnsureProductCategoriesSchemaAsync(context);

        // Check if data already exists
        if (context.Products.Any())
        {
            return;
        }

        // Add sample categories
        var beverages = new ProductCategory { Name = "Beverages", Description = "Hot and cold drinks" };
        var food = new ProductCategory { Name = "Food", Description = "Meals and snacks" };
        var desserts = new ProductCategory { Name = "Desserts", Description = "Sweet treats" };

        context.ProductCategories.AddRange(beverages, food, desserts);
        await context.SaveChangesAsync();

        // Add sample units
        var piece = new Unit { Name = "Piece", Abbreviation = "pc" };
        var cup = new Unit { Name = "Cup", Abbreviation = "cup" };
        var glass = new Unit { Name = "Glass", Abbreviation = "glass" };

        context.Units.AddRange(piece, cup, glass);
        await context.SaveChangesAsync();

        // Add sample products
        var products = new List<Product>
        {
            new Product
            {
                Code = "COF001",
                Name = "Coffee",
                Description = "Hot brewed coffee",
                CostPrice = 15,
                SellingPrice = 35,
                CategoryId = beverages.Id,
                UnitId = cup.Id,
                IsActive = true,
                IsTaxable = true,
                ImageUrl = "coffee.png"
            },
            new Product
            {
                Code = "TEA001",
                Name = "Iced Tea",
                Description = "Refreshing iced tea",
                CostPrice = 10,
                SellingPrice = 25,
                CategoryId = beverages.Id,
                UnitId = glass.Id,
                IsActive = true,
                IsTaxable = true,
                ImageUrl = "icetea.png"
            },
            new Product
            {
                Code = "BUR001",
                Name = "Burger",
                Description = "Classic beef burger",
                CostPrice = 50,
                SellingPrice = 85,
                CategoryId = food.Id,
                UnitId = piece.Id,
                IsActive = true,
                IsTaxable = true,
                ImageUrl = "burger.png"
            },
            new Product
            {
                Code = "FRI001",
                Name = "French Fries",
                Description = "Crispy french fries",
                CostPrice = 20,
                SellingPrice = 45,
                CategoryId = food.Id,
                UnitId = piece.Id,
                IsActive = true,
                IsTaxable = true,
                ImageUrl = "fries.png"
            },
            new Product
            {
                Code = "CAK001",
                Name = "Chocolate Cake",
                Description = "Rich chocolate cake slice",
                CostPrice = 30,
                SellingPrice = 65,
                CategoryId = desserts.Id,
                UnitId = piece.Id,
                IsActive = true,
                IsTaxable = true,
                ImageUrl = "cake.png"
            },
            new Product
            {
                Code = "ICE001",
                Name = "Ice Cream",
                Description = "Vanilla ice cream scoop",
                CostPrice = 15,
                SellingPrice = 40,
                CategoryId = desserts.Id,
                UnitId = piece.Id,
                IsActive = true,
                IsTaxable = true,
                ImageUrl = "icecream.png"
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // Add sample stock
        foreach (var product in products)
        {
            context.ProductStocks.Add(new ProductStock
            {
                ProductId = product.Id,
                BaseQuantity = 100,
                DisplayQuantity = 100,
                ReorderLevel = 10,
                MaxStock = 200
            });
        }

        await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DatabaseSeeder] Error: {ex.Message}");
            throw;
        }
    }

    public static async Task EnsureUserManagementSchemaAsync(LocalDbContext context)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS "Users" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
                    "FullName" TEXT NOT NULL,
                    "Username" TEXT NOT NULL,
                    "Email" TEXT NOT NULL,
                    "PasswordHash" TEXT NOT NULL,
                    "Role" TEXT NOT NULL,
                    "Branch" TEXT NOT NULL,
                    "IsAdmin" INTEGER NOT NULL,
                    "HasTwoFactor" INTEGER NOT NULL,
                    "IsActive" INTEGER NOT NULL,
                    "LastLoginAt" TEXT NULL,
                    "CreatedAt" TEXT NOT NULL,
                    "UpdatedAt" TEXT NOT NULL,
                    "IsDeleted" INTEGER NOT NULL
                );
                """);

            await context.Database.ExecuteSqlRawAsync("""
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Username"
                ON "Users" ("Username")
                WHERE "IsDeleted" = 0;
                """);

            await context.Database.ExecuteSqlRawAsync("""
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email"
                ON "Users" ("Email")
                WHERE "IsDeleted" = 0;
                """);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EnsureUserManagementSchemaAsync] Error: {ex.Message}");
        }
    }

    public static async Task EnsureUnitsSchemaAsync(LocalDbContext context)
    {
        try
        {
            // Ensure Units table has all required columns
            await context.Database.ExecuteSqlRawAsync("""
                CREATE TABLE IF NOT EXISTS "Units" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Units" PRIMARY KEY AUTOINCREMENT,
                    "Name" TEXT NOT NULL,
                    "Abbreviation" TEXT NOT NULL,
                    "BaseUnitId" INTEGER NULL,
                    "ConversionFactor" REAL NOT NULL,
                    "IsActive" INTEGER NOT NULL,
                    "IsDeleted" INTEGER NOT NULL,
                    "CreatedAt" TEXT NOT NULL,
                    "UpdatedAt" TEXT NOT NULL
                );
                """);

            // Add missing columns for existing databases
            await context.Database.ExecuteSqlRawAsync("ALTER TABLE \"Units\" ADD COLUMN \"BaseUnitId\" INTEGER NULL;");
            await context.Database.ExecuteSqlRawAsync("ALTER TABLE \"Units\" ADD COLUMN \"ConversionFactor\" REAL NOT NULL DEFAULT 1.0;");
        }
        catch (Exception ex)
        {
            // Ignore errors if column already exists
            System.Diagnostics.Debug.WriteLine($"[EnsureUnitsSchemaAsync] Error: {ex.Message}");
        }
    }

    public static async Task EnsureProductCategoriesSchemaAsync(LocalDbContext context)
    {
        try
        {
            // Add SortOrder column if it doesn't exist
            await context.Database.ExecuteSqlRawAsync("ALTER TABLE \"ProductCategories\" ADD COLUMN \"SortOrder\" INTEGER NOT NULL DEFAULT 0;");
        }
        catch (Exception ex)
        {
            // Ignore errors if column already exists
            System.Diagnostics.Debug.WriteLine($"[EnsureProductCategoriesSchemaAsync] Error: {ex.Message}");
        }
    }
}
