using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class ProductStockTests : TestBase
{
    [Fact]
    public async Task CanCreateProductStock()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var category = new ProductCategory { Name = "Food" };
        var baseUnit = new Unit { Name = "Kilogram", Abbreviation = "KG", IsBase = true };
        var displayUnit = new Unit { Name = "Gram", Abbreviation = "G", BaseUnit = baseUnit, ConversionFactor = 0.001m };
        var product = new Product { Code = "PROD001", Name = "Rice", Category = category, Unit = baseUnit };
        
        var productStock = new ProductStock
        {
            Product = product,
            Branch = branch,
            BaseUnit = baseUnit,
            DisplayUnit = displayUnit,
            BaseQuantity = 100.00m,
            DisplayQuantity = 100000.00m,
            ReorderLevel = 20.00m
        };

        // Act
        Context.Branches.Add(branch);
        Context.ProductCategories.Add(category);
        Context.Units.Add(baseUnit);
        Context.Units.Add(displayUnit);
        Context.Products.Add(product);
        Context.ProductStocks.Add(productStock);
        await Context.SaveChangesAsync();

        // Assert
        var savedStock = await Context.ProductStocks
            .Include(ps => ps.Product)
            .Include(ps => ps.Branch)
            .FirstOrDefaultAsync(ps => ps.Product.Code == "PROD001");
        
        Assert.NotNull(savedStock);
        Assert.Equal(100.00m, savedStock.BaseQuantity);
        Assert.Equal("Rice", savedStock.Product.Name);
    }

    [Fact]
    public async Task CanUpdateProductStock()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var category = new ProductCategory { Name = "Food" };
        var unit = new Unit { Name = "Piece", Abbreviation = "PCS", IsBase = true };
        var product = new Product { Code = "PROD001", Name = "Item", Category = category, Unit = unit };
        var stock = new ProductStock { Product = product, Branch = branch, BaseUnit = unit, DisplayUnit = unit, BaseQuantity = 50.00m };
        
        Context.Branches.Add(branch);
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        Context.ProductStocks.Add(stock);
        await Context.SaveChangesAsync();

        // Act
        stock.BaseQuantity = 75.00m;
        stock.ReorderLevel = 15.00m;
        await Context.SaveChangesAsync();

        // Assert
        var updatedStock = await Context.ProductStocks.FirstOrDefaultAsync(ps => ps.Product.Code == "PROD001");
        Assert.Equal(75.00m, updatedStock?.BaseQuantity);
        Assert.Equal(15.00m, updatedStock?.ReorderLevel);
    }

    [Fact]
    public async Task CanDeleteProductStock_SoftDelete()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var category = new ProductCategory { Name = "Food" };
        var unit = new Unit { Name = "Piece", Abbreviation = "PCS", IsBase = true };
        var product = new Product { Code = "PROD001", Name = "Item", Category = category, Unit = unit };
        var stock = new ProductStock { Product = product, Branch = branch, BaseUnit = unit, DisplayUnit = unit };
        
        Context.Branches.Add(branch);
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        Context.ProductStocks.Add(stock);
        await Context.SaveChangesAsync();

        // Act
        stock.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedStock = await Context.ProductStocks.FirstOrDefaultAsync(ps => ps.Product.Code == "PROD001");
        Assert.Null(deletedStock);
    }
}
