using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class TaxTests : TestBase
{
    [Fact]
    public async Task CanCreateTax()
    {
        // Arrange
        var tax = new Tax
        {
            Name = "VAT",
            Rate = 12.00m,
            IsActive = true
        };

        // Act
        Context.Taxes.Add(tax);
        await Context.SaveChangesAsync();

        // Assert
        var savedTax = await Context.Taxes.FirstOrDefaultAsync(t => t.Name == "VAT");
        Assert.NotNull(savedTax);
        Assert.Equal("VAT", savedTax.Name);
        Assert.Equal(12.00m, savedTax.Rate);
    }

    [Fact]
    public async Task CanUpdateTax()
    {
        // Arrange
        var tax = new Tax { Name = "VAT", Rate = 12.00m };
        Context.Taxes.Add(tax);
        await Context.SaveChangesAsync();

        // Act
        tax.Rate = 15.00m;
        tax.IsActive = false;
        await Context.SaveChangesAsync();

        // Assert
        var updatedTax = await Context.Taxes.FirstOrDefaultAsync(t => t.Name == "VAT");
        Assert.Equal(15.00m, updatedTax?.Rate);
        Assert.False(updatedTax?.IsActive);
    }

    [Fact]
    public async Task CanDeleteTax_SoftDelete()
    {
        // Arrange
        var tax = new Tax { Name = "Test" };
        Context.Taxes.Add(tax);
        await Context.SaveChangesAsync();

        // Act
        tax.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedTax = await Context.Taxes.FirstOrDefaultAsync(t => t.Name == "Test");
        Assert.Null(deletedTax);
    }
}
