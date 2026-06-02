using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class StockMovementTests : TestBase
{
    [Fact]
    public async Task CanCreateStockMovement()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var category = new ProductCategory { Name = "Food" };
        var unit = new Unit { Name = "Piece", Abbreviation = "PCS", IsBase = true };
        var product = new Product { Code = "PROD001", Name = "Item", Category = category, Unit = unit };
        var user = new User { Username = "testuser", Email = "test@example.com" };
        
        var stockMovement = new StockMovement
        {
            Product = product,
            Branch = branch,
            Unit = unit,
            BaseUnit = unit,
            MovementType = "IN",
            Quantity = 50.00m,
            BaseQuantity = 50.00m,
            ReferenceType = "PURCHASE_ORDER",
            ReferenceId = 1,
            Notes = "Initial stock"
        };

        // Act
        Context.Branches.Add(branch);
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        Context.Users.Add(user);
        Context.StockMovements.Add(stockMovement);
        await Context.SaveChangesAsync();

        // Assert
        var savedMovement = await Context.StockMovements
            .Include(sm => sm.Product)
            .FirstOrDefaultAsync(sm => sm.MovementType == "IN");
        
        Assert.NotNull(savedMovement);
        Assert.Equal("IN", savedMovement.MovementType);
        Assert.Equal(50.00m, savedMovement.Quantity);
    }

    [Fact]
    public async Task CanUpdateStockMovement()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var category = new ProductCategory { Name = "Food" };
        var unit = new Unit { Name = "Piece", Abbreviation = "PCS", IsBase = true };
        var product = new Product { Code = "PROD001", Name = "Item", Category = category, Unit = unit };
        var movement = new StockMovement { Product = product, Branch = branch, Unit = unit, BaseUnit = unit, MovementType = "IN", Quantity = 50.00m };
        
        Context.Branches.Add(branch);
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        Context.StockMovements.Add(movement);
        await Context.SaveChangesAsync();

        // Act
        movement.Notes = "Updated notes";
        await Context.SaveChangesAsync();

        // Assert
        var updatedMovement = await Context.StockMovements.FirstOrDefaultAsync(sm => sm.MovementType == "IN");
        Assert.Equal("Updated notes", updatedMovement?.Notes);
    }

    [Fact]
    public async Task CanDeleteStockMovement_SoftDelete()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var category = new ProductCategory { Name = "Food" };
        var unit = new Unit { Name = "Piece", Abbreviation = "PCS", IsBase = true };
        var product = new Product { Code = "PROD001", Name = "Item", Category = category, Unit = unit };
        var movement = new StockMovement { Product = product, Branch = branch, Unit = unit, BaseUnit = unit, MovementType = "IN" };
        
        Context.Branches.Add(branch);
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        Context.StockMovements.Add(movement);
        await Context.SaveChangesAsync();

        // Act
        movement.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedMovement = await Context.StockMovements.FirstOrDefaultAsync(sm => sm.MovementType == "IN");
        Assert.Null(deletedMovement);
    }
}
