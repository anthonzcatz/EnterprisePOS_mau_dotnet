using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class DeliveryOrderTests : TestBase
{
    [Fact]
    public async Task CanCreateDeliveryOrder()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var terminal = new PosTerminal { Name = "Terminal 1", Code = "T1", Branch = branch };
        var customer = new Customer { Code = "CUST001", Name = "John Doe" };
        var driver = new DeliveryDriver { Name = "Driver 1", Branch = branch };
        var deliveryAddress = new DeliveryAddress { Customer = customer, AddressLine1 = "123 Main St", City = "City", IsDefault = true };
        var sale = new Sale { Branch = branch, Terminal = terminal, Customer = customer, SaleNumber = "SALE001", TotalAmount = 100.00m };
        
        var deliveryOrder = new DeliveryOrder
        {
            Sale = sale,
            Branch = branch,
            Driver = driver,
            DeliveryAddress = deliveryAddress,
            ScheduledDate = DateTime.UtcNow,
            Status = "PENDING"
        };

        // Act
        Context.Branches.Add(branch);
        Context.PosTerminals.Add(terminal);
        Context.Customers.Add(customer);
        Context.DeliveryDrivers.Add(driver);
        Context.DeliveryAddresses.Add(deliveryAddress);
        Context.Sales.Add(sale);
        Context.DeliveryOrders.Add(deliveryOrder);
        await Context.SaveChangesAsync();

        // Assert
        var savedOrder = await Context.DeliveryOrders
            .Include(d => d.Sale)
            .Include(d => d.DeliveryAddress)
            .FirstOrDefaultAsync(d => d.DeliveryAddress.AddressLine1 == "123 Main St");
        
        Assert.NotNull(savedOrder);
        Assert.Equal("123 Main St", savedOrder.DeliveryAddress.AddressLine1);
        Assert.Equal("SALE001", savedOrder.Sale.SaleNumber);
    }

    [Fact]
    public async Task CanUpdateDeliveryOrder()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var terminal = new PosTerminal { Name = "Terminal 1", Code = "T1", Branch = branch };
        var customer = new Customer { Code = "CUST001", Name = "John Doe" };
        var deliveryAddress = new DeliveryAddress { Customer = customer, AddressLine1 = "123 Main St", City = "City", IsDefault = true };
        var sale = new Sale { Branch = branch, Terminal = terminal, Customer = customer, SaleNumber = "SALE001", TotalAmount = 100.00m };
        var order = new DeliveryOrder { Sale = sale, Branch = branch, DeliveryAddress = deliveryAddress, Status = "PENDING" };
        
        Context.Branches.Add(branch);
        Context.PosTerminals.Add(terminal);
        Context.Customers.Add(customer);
        Context.DeliveryAddresses.Add(deliveryAddress);
        Context.Sales.Add(sale);
        Context.DeliveryOrders.Add(order);
        await Context.SaveChangesAsync();

        // Act
        order.Status = "DELIVERED";
        await Context.SaveChangesAsync();

        // Assert
        var updatedOrder = await Context.DeliveryOrders.FirstOrDefaultAsync(d => d.SaleId == sale.Id);
        Assert.Equal("DELIVERED", updatedOrder?.Status);
    }

    [Fact]
    public async Task CanDeleteDeliveryOrder_SoftDelete()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var terminal = new PosTerminal { Name = "Terminal 1", Code = "T1", Branch = branch };
        var customer = new Customer { Code = "CUST001", Name = "John Doe" };
        var deliveryAddress = new DeliveryAddress { Customer = customer, AddressLine1 = "123 Main St", City = "City", IsDefault = true };
        var sale = new Sale { Branch = branch, Terminal = terminal, Customer = customer, SaleNumber = "SALE001", TotalAmount = 100.00m };
        var order = new DeliveryOrder { Sale = sale, Branch = branch, DeliveryAddress = deliveryAddress };
        
        Context.Branches.Add(branch);
        Context.PosTerminals.Add(terminal);
        Context.Customers.Add(customer);
        Context.DeliveryAddresses.Add(deliveryAddress);
        Context.Sales.Add(sale);
        Context.DeliveryOrders.Add(order);
        await Context.SaveChangesAsync();

        // Act
        order.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedOrder = await Context.DeliveryOrders.FirstOrDefaultAsync(d => d.SaleId == sale.Id);
        Assert.Null(deletedOrder);
    }
}
