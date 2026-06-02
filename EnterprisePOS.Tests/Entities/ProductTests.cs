using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class ProductTests : TestBase
{
    [Fact]
    public async Task CanCreateProduct()
    {
        // Arrange
        var category = new ProductCategory { Name = "Electronics" };
        var unit = new Unit { Name = "Piece", Abbreviation = "PCS" };
        
        var product = new Product
        {
            Code = "PROD001",
            Name = "Test Product",
            Description = "Test description",
            Category = category,
            Unit = unit,
            CostPrice = 10.00m,
            SellingPrice = 15.00m,
            IsActive = true,
            IsTaxable = true
        };

        // Act
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        await Context.SaveChangesAsync();

        // Assert
        var savedProduct = await Context.Products.FirstOrDefaultAsync(p => p.Code == "PROD001");
        Assert.NotNull(savedProduct);
        Assert.Equal("Test Product", savedProduct.Name);
        Assert.Equal(15.00m, savedProduct.SellingPrice);
    }

    [Fact]
    public async Task CanUpdateProduct()
    {
        // Arrange
        var category = new ProductCategory { Name = "Electronics" };
        var unit = new Unit { Name = "Piece", Abbreviation = "PCS" };
        var product = new Product { Code = "PROD001", Name = "Old Name", Category = category, Unit = unit };
        
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        await Context.SaveChangesAsync();

        // Act
        product.Name = "New Name";
        product.SellingPrice = 20.00m;
        await Context.SaveChangesAsync();

        // Assert
        var updatedProduct = await Context.Products.FirstOrDefaultAsync(p => p.Code == "PROD001");
        Assert.Equal("New Name", updatedProduct?.Name);
        Assert.Equal(20.00m, updatedProduct?.SellingPrice);
    }

    [Fact]
    public async Task CanDeleteProduct_SoftDelete()
    {
        // Arrange
        var category = new ProductCategory { Name = "Electronics" };
        var unit = new Unit { Name = "Piece", Abbreviation = "PCS" };
        var product = new Product { Code = "PROD001", Name = "Test", Category = category, Unit = unit };
        
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        await Context.SaveChangesAsync();

        // Act
        product.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedProduct = await Context.Products.FirstOrDefaultAsync(p => p.Code == "PROD001");
        Assert.Null(deletedProduct);
    }

    [Fact]
    public async Task CanGetProductWithCategoryAndUnit()
    {
        // Arrange
        var category = new ProductCategory { Name = "Food" };
        var unit = new Unit { Name = "Kilogram", Abbreviation = "KG" };
        var product = new Product { Code = "FOOD001", Name = "Rice", Category = category, Unit = unit };

        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        await Context.SaveChangesAsync();

        // Act
        var productWithDetails = await Context.Products
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .FirstOrDefaultAsync(p => p.Code == "FOOD001");

        // Assert
        Assert.NotNull(productWithDetails);
        Assert.Equal("Food", productWithDetails.Category?.Name);
        Assert.Equal("Kilogram", productWithDetails.Unit?.Name);
    }
}
