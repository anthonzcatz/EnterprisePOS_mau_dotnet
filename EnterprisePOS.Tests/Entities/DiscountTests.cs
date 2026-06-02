using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class DiscountTests : TestBase
{
    [Fact]
    public async Task CanCreateDiscount()
    {
        // Arrange
        var discount = new Discount
        {
            Name = "10% Off",
            Code = "DISC10",
            Type = "PERCENTAGE",
            Value = 10.00m,
            IsActive = true
        };

        // Act
        Context.Discounts.Add(discount);
        await Context.SaveChangesAsync();

        // Assert
        var savedDiscount = await Context.Discounts.FirstOrDefaultAsync(d => d.Code == "DISC10");
        Assert.NotNull(savedDiscount);
        Assert.Equal("10% Off", savedDiscount.Name);
        Assert.Equal("PERCENTAGE", savedDiscount.Type);
    }

    [Fact]
    public async Task CanUpdateDiscount()
    {
        // Arrange
        var discount = new Discount { Name = "10% Off", Code = "DISC10", Type = "PERCENTAGE", Value = 10.00m };
        Context.Discounts.Add(discount);
        await Context.SaveChangesAsync();

        // Act
        discount.Value = 15.00m;
        discount.IsActive = false;
        await Context.SaveChangesAsync();

        // Assert
        var updatedDiscount = await Context.Discounts.FirstOrDefaultAsync(d => d.Code == "DISC10");
        Assert.Equal(15.00m, updatedDiscount?.Value);
    }

    [Fact]
    public async Task CanDeleteDiscount_SoftDelete()
    {
        // Arrange
        var discount = new Discount { Name = "Test", Code = "TEST" };
        Context.Discounts.Add(discount);
        await Context.SaveChangesAsync();

        // Act
        discount.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedDiscount = await Context.Discounts.FirstOrDefaultAsync(d => d.Code == "TEST");
        Assert.Null(deletedDiscount);
    }
}
