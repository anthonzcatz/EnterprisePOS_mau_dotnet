using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class SupplierTests : TestBase
{
    [Fact]
    public async Task CanCreateSupplier()
    {
        // Arrange
        var supplier = new Supplier
        {
            Name = "ABC Supplier",
            ContactPerson = "John Smith",
            Phone = "555-1234",
            Email = "supplier@example.com",
            Address = "123 Supplier St",
            IsActive = true
        };

        // Act
        Context.Suppliers.Add(supplier);
        await Context.SaveChangesAsync();

        // Assert
        var savedSupplier = await Context.Suppliers.FirstOrDefaultAsync(s => s.Name == "ABC Supplier");
        Assert.NotNull(savedSupplier);
        Assert.Equal("ABC Supplier", savedSupplier.Name);
        Assert.Equal("John Smith", savedSupplier.ContactPerson);
    }

    [Fact]
    public async Task CanUpdateSupplier()
    {
        // Arrange
        var supplier = new Supplier { Name = "ABC Supplier" };
        Context.Suppliers.Add(supplier);
        await Context.SaveChangesAsync();

        // Act
        supplier.Phone = "555-9999";
        supplier.Address = "Updated Address";
        await Context.SaveChangesAsync();

        // Assert
        var updatedSupplier = await Context.Suppliers.FirstOrDefaultAsync(s => s.Name == "ABC Supplier");
        Assert.Equal("555-9999", updatedSupplier?.Phone);
        Assert.Equal("Updated Address", updatedSupplier?.Address);
    }

    [Fact]
    public async Task CanDeleteSupplier_SoftDelete()
    {
        // Arrange
        var supplier = new Supplier { Name = "Test Supplier" };
        Context.Suppliers.Add(supplier);
        await Context.SaveChangesAsync();

        // Act
        supplier.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedSupplier = await Context.Suppliers.FirstOrDefaultAsync(s => s.Name == "Test Supplier");
        Assert.Null(deletedSupplier);
    }

    [Fact]
    public async Task CanGetSupplierWithPurchaseOrders()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var supplier = new Supplier { Name = "ABC Supplier" };
        var purchaseOrder = new PurchaseOrder { OrderNumber = "PO001", Supplier = supplier, Branch = branch };

        Context.Branches.Add(branch);
        Context.Suppliers.Add(supplier);
        Context.PurchaseOrders.Add(purchaseOrder);
        await Context.SaveChangesAsync();

        // Act
        var supplierWithOrders = await Context.Suppliers
            .Include(s => s.PurchaseOrders)
            .FirstOrDefaultAsync(s => s.Name == "ABC Supplier");

        // Assert
        Assert.NotNull(supplierWithOrders);
        Assert.Single(supplierWithOrders.PurchaseOrders);
        Assert.Equal("PO001", supplierWithOrders.PurchaseOrders.First().OrderNumber);
    }
}
