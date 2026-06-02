using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class CustomerTests : TestBase
{
    [Fact]
    public async Task CanCreateCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Code = "CUST001",
            Name = "John Doe",
            Phone = "555-1234",
            Email = "john@example.com",
            Address = "123 Main St",
            LoyaltyPoints = 100,
            CreditLimit = 1000.00m,
            CurrentBalance = 500.00m,
            IsActive = true
        };

        // Act
        Context.Customers.Add(customer);
        await Context.SaveChangesAsync();

        // Assert
        var savedCustomer = await Context.Customers.FirstOrDefaultAsync(c => c.Code == "CUST001");
        Assert.NotNull(savedCustomer);
        Assert.Equal("John Doe", savedCustomer.Name);
        Assert.Equal(100, savedCustomer.LoyaltyPoints);
    }

    [Fact]
    public async Task CanUpdateCustomer()
    {
        // Arrange
        var customer = new Customer { Code = "CUST001", Name = "Jane Doe" };
        Context.Customers.Add(customer);
        await Context.SaveChangesAsync();

        // Act
        customer.Phone = "555-9999";
        customer.LoyaltyPoints = 200;
        await Context.SaveChangesAsync();

        // Assert
        var updatedCustomer = await Context.Customers.FirstOrDefaultAsync(c => c.Code == "CUST001");
        Assert.Equal("555-9999", updatedCustomer?.Phone);
        Assert.Equal(200, updatedCustomer?.LoyaltyPoints);
    }

    [Fact]
    public async Task CanDeleteCustomer_SoftDelete()
    {
        // Arrange
        var customer = new Customer { Code = "CUST001", Name = "Test Customer" };
        Context.Customers.Add(customer);
        await Context.SaveChangesAsync();

        // Act
        customer.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedCustomer = await Context.Customers.FirstOrDefaultAsync(c => c.Code == "CUST001");
        Assert.Null(deletedCustomer);
    }

    [Fact]
    public async Task CanGetCustomerWithSales()
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
            TotalAmount = 110.00m
        };

        Context.Branches.Add(branch);
        Context.PosTerminals.Add(terminal);
        Context.Customers.Add(customer);
        Context.Sales.Add(sale);
        await Context.SaveChangesAsync();

        // Act
        var customerWithSales = await Context.Customers
            .Include(c => c.Sales)
            .FirstOrDefaultAsync(c => c.Code == "CUST001");

        // Assert
        Assert.NotNull(customerWithSales);
        Assert.Single(customerWithSales.Sales);
        Assert.Equal("SALE001", customerWithSales.Sales.First().SaleNumber);
    }
}
