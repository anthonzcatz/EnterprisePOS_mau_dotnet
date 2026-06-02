using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class PaymentMethodTests : TestBase
{
    [Fact]
    public async Task CanCreatePaymentMethod()
    {
        // Arrange
        var paymentMethod = new PaymentMethod
        {
            Name = "Cash",
            Code = "CASH",
            IsActive = true
        };

        // Act
        Context.PaymentMethods.Add(paymentMethod);
        await Context.SaveChangesAsync();

        // Assert
        var savedMethod = await Context.PaymentMethods.FirstOrDefaultAsync(pm => pm.Code == "CASH");
        Assert.NotNull(savedMethod);
        Assert.Equal("Cash", savedMethod.Name);
        Assert.True(savedMethod.IsActive);
    }

    [Fact]
    public async Task CanUpdatePaymentMethod()
    {
        // Arrange
        var paymentMethod = new PaymentMethod { Name = "Cash", Code = "CASH" };
        Context.PaymentMethods.Add(paymentMethod);
        await Context.SaveChangesAsync();

        // Act
        paymentMethod.IsActive = false;
        await Context.SaveChangesAsync();

        // Assert
        var updatedMethod = await Context.PaymentMethods.FirstOrDefaultAsync(pm => pm.Code == "CASH");
        Assert.False(updatedMethod?.IsActive);
    }

    [Fact]
    public async Task CanDeletePaymentMethod_SoftDelete()
    {
        // Arrange
        var paymentMethod = new PaymentMethod { Name = "Test", Code = "TEST" };
        Context.PaymentMethods.Add(paymentMethod);
        await Context.SaveChangesAsync();

        // Act
        paymentMethod.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedMethod = await Context.PaymentMethods.FirstOrDefaultAsync(pm => pm.Code == "TEST");
        Assert.Null(deletedMethod);
    }
}
