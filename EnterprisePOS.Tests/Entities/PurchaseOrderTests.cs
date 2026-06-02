using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class PurchaseOrderTests : TestBase
{
    [Fact]
    public async Task CanCreatePurchaseOrder()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var supplier = new Supplier { Name = "ABC Supplier" };
        var user = new User { Username = "testuser", Email = "test@example.com" };
        
        var purchaseOrder = new PurchaseOrder
        {
            OrderNumber = "PO001",
            Supplier = supplier,
            Branch = branch,
            OrderDate = DateTime.UtcNow,
            ExpectedDate = DateTime.UtcNow.AddDays(7),
            Status = "PENDING",
            TotalAmount = 1100.00m,
            Creator = user
        };

        // Act
        Context.Branches.Add(branch);
        Context.Suppliers.Add(supplier);
        Context.Users.Add(user);
        Context.PurchaseOrders.Add(purchaseOrder);
        await Context.SaveChangesAsync();

        // Assert
        var savedOrder = await Context.PurchaseOrders
            .Include(po => po.Supplier)
            .FirstOrDefaultAsync(po => po.OrderNumber == "PO001");
        
        Assert.NotNull(savedOrder);
        Assert.Equal("PO001", savedOrder.OrderNumber);
        Assert.Equal("ABC Supplier", savedOrder.Supplier.Name);
    }

    [Fact]
    public async Task CanUpdatePurchaseOrder()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var supplier = new Supplier { Name = "ABC Supplier" };
        var order = new PurchaseOrder { OrderNumber = "PO001", Supplier = supplier, Branch = branch, Status = "PENDING" };
        
        Context.Branches.Add(branch);
        Context.Suppliers.Add(supplier);
        Context.PurchaseOrders.Add(order);
        await Context.SaveChangesAsync();

        // Act
        order.Status = "RECEIVED";
        order.TotalAmount = 1200.00m;
        await Context.SaveChangesAsync();

        // Assert
        var updatedOrder = await Context.PurchaseOrders.FirstOrDefaultAsync(po => po.OrderNumber == "PO001");
        Assert.Equal("RECEIVED", updatedOrder?.Status);
        Assert.Equal(1200.00m, updatedOrder?.TotalAmount);
    }

    [Fact]
    public async Task CanDeletePurchaseOrder_SoftDelete()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var supplier = new Supplier { Name = "ABC Supplier" };
        var order = new PurchaseOrder { OrderNumber = "PO001", Supplier = supplier, Branch = branch };
        
        Context.Branches.Add(branch);
        Context.Suppliers.Add(supplier);
        Context.PurchaseOrders.Add(order);
        await Context.SaveChangesAsync();

        // Act
        order.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedOrder = await Context.PurchaseOrders.FirstOrDefaultAsync(po => po.OrderNumber == "PO001");
        Assert.Null(deletedOrder);
    }

    [Fact]
    public async Task CanGetPurchaseOrderWithItems()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var supplier = new Supplier { Name = "ABC Supplier" };
        var category = new ProductCategory { Name = "Food" };
        var unit = new Unit { Name = "Piece", Abbreviation = "PCS", IsBase = true };
        var product = new Product { Code = "PROD001", Name = "Item", Category = category, Unit = unit };
        
        var order = new PurchaseOrder { OrderNumber = "PO001", Supplier = supplier, Branch = branch };
        var orderItem = new PurchaseOrderItem
        {
            PurchaseOrder = order,
            Product = product,
            Unit = unit,
            BaseUnit = unit,
            Quantity = 10,
            BaseQuantity = 10,
            UnitPrice = 50.00m,
            TotalPrice = 500.00m
        };

        Context.Branches.Add(branch);
        Context.Suppliers.Add(supplier);
        Context.ProductCategories.Add(category);
        Context.Units.Add(unit);
        Context.Products.Add(product);
        Context.PurchaseOrders.Add(order);
        Context.PurchaseOrderItems.Add(orderItem);
        await Context.SaveChangesAsync();

        // Act
        var orderWithItems = await Context.PurchaseOrders
            .Include(po => po.PurchaseOrderItems)
            .ThenInclude(poi => poi.Product)
            .FirstOrDefaultAsync(po => po.OrderNumber == "PO001");

        // Assert
        Assert.NotNull(orderWithItems);
        Assert.Single(orderWithItems.PurchaseOrderItems);
        Assert.Equal("Item", orderWithItems.PurchaseOrderItems.First().Product.Name);
    }
}
