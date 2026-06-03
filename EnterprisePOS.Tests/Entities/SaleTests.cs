using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class SaleTests : TestBase
{
    [Fact]
    public async Task CanCreateSale()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var terminal = new PosTerminal { Name = "Terminal 1", Code = "T1", Branch = branch };
        var customer = new Customer { Code = "CUST001", Name = "John Doe" };
        
        var sale = new Sale
        {
            Branch = branch,
            Terminal = terminal,
            Customer = customer,
            SaleNumber = "SALE001",
            SaleDate = DateTime.UtcNow,
            Subtotal = 100.00m,
            DiscountAmount = 10.00m,
            TaxAmount = 9.00m,
            TotalAmount = 99.00m,
            PaidAmount = 100.00m,
            ChangeAmount = 1.00m,
            Status = "COMPLETED"
        };

        // Act
        Context.Branches.Add(branch);
        Context.PosTerminals.Add(terminal);
        Context.Customers.Add(customer);
        Context.Sales.Add(sale);
        await Context.SaveChangesAsync();

        // Assert
        var savedSale = await Context.Sales.FirstOrDefaultAsync(s => s.SaleNumber == "SALE001");
        Assert.NotNull(savedSale);
        Assert.Equal("SALE001", savedSale.SaleNumber);
        Assert.Equal(99.00m, savedSale.TotalAmount);
    }

    [Fact]
    public async Task CanUpdateSale()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var terminal = new PosTerminal { Name = "T1", Code = "T1", Branch = branch };
        var sale = new Sale { Branch = branch, Terminal = terminal, SaleNumber = "S1", TotalAmount = 100 };
        
        Context.Branches.Add(branch);
        Context.PosTerminals.Add(terminal);
        Context.Sales.Add(sale);
        await Context.SaveChangesAsync();

        // Act
        sale.Status = "REFUNDED";
        sale.TotalAmount = 90.00m;
        await Context.SaveChangesAsync();

        // Assert
        var updatedSale = await Context.Sales.FirstOrDefaultAsync(s => s.SaleNumber == "SALE001");
        Assert.Equal("REFUNDED", updatedSale?.Status);
        Assert.Equal(90.00m, updatedSale?.TotalAmount);
    }

    [Fact]
    public async Task CanDeleteSale_SoftDelete()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var terminal = new PosTerminal { Name = "Terminal 1", Code = "T1", Branch = branch };
        var sale = new Sale { Branch = branch, Terminal = terminal, SaleNumber = "SALE001" };
        
        Context.Branches.Add(branch);
        Context.PosTerminals.Add(terminal);
        Context.Sales.Add(sale);
        await Context.SaveChangesAsync();

        // Act
        sale.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedSale = await Context.Sales.FirstOrDefaultAsync(s => s.SaleNumber == "SALE001");
        Assert.Null(deletedSale);
    }

    [Fact]
    public async Task CanGetSaleWithItems()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var terminal = new PosTerminal { Name = "Terminal 1", Code = "T1", Branch = branch };
        var category = new ProductCategory { Name = "Food" };
        var unit = new Unit { Name = "Piece", Abbreviation = "PCS" };
        var product = new Product { Code = "PROD001", Name = "Burger", Category = category, Unit = unit };
        
        var sale = new Sale { Branch = branch, Terminal = terminal, SaleNumber = "SALE001", TotalAmount = 50.00m };
        var saleItem = new SaleItem
        {
            Sale = sale,
            Product = product,
            Unit = unit,
            BaseUnit = unit,
            Quantity = 2,
            BaseQuantity = 2,
            UnitPrice = 25.00m,
            TotalPrice = 50.00m
        };

        Context.Branches.Add(branch);
        Context.PosTerminals.Add(terminal);
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        Context.Sales.Add(sale);
        Context.SaleItems.Add(saleItem);
        await Context.SaveChangesAsync();

        // Act
        var saleWithItems = await Context.Sales
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s => s.SaleNumber == "SALE001");

        // Assert
        Assert.NotNull(saleWithItems);
        Assert.Single(saleWithItems.SaleItems);
        Assert.Equal("Burger", saleWithItems.SaleItems.First().Product.Name);
    }
}
