using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class GiftCardTests : TestBase
{
    [Fact]
    public async Task CanCreateGiftCard()
    {
        // Arrange
        var customer = new Customer { Code = "CUST001", Name = "John Doe" };
        var giftCard = new GiftCard
        {
            CardNumber = "GC123456789",
            Pin = "1234",
            Customer = customer,
            InitialBalance = 500.00m,
            CurrentBalance = 500.00m,
            IssueDate = DateTime.UtcNow,
            Status = "ACTIVE"
        };

        // Act
        Context.Customers.Add(customer);
        Context.GiftCards.Add(giftCard);
        await Context.SaveChangesAsync();

        // Assert
        var savedCard = await Context.GiftCards
            .Include(gc => gc.Customer)
            .FirstOrDefaultAsync(gc => gc.CardNumber == "GC123456789");
        
        Assert.NotNull(savedCard);
        Assert.Equal("GC123456789", savedCard.CardNumber);
        Assert.Equal(500.00m, savedCard.CurrentBalance);
    }

    [Fact]
    public async Task CanUpdateGiftCard()
    {
        // Arrange
        var giftCard = new GiftCard { CardNumber = "GC123456789", InitialBalance = 500.00m, CurrentBalance = 500.00m };
        Context.GiftCards.Add(giftCard);
        await Context.SaveChangesAsync();

        // Act
        giftCard.CurrentBalance = 300.00m;
        giftCard.Status = "USED";
        await Context.SaveChangesAsync();

        // Assert
        var updatedCard = await Context.GiftCards.FirstOrDefaultAsync(gc => gc.CardNumber == "GC123456789");
        Assert.Equal(300.00m, updatedCard?.CurrentBalance);
        Assert.Equal("USED", updatedCard?.Status);
    }

    [Fact]
    public async Task CanDeleteGiftCard_SoftDelete()
    {
        // Arrange
        var giftCard = new GiftCard { CardNumber = "GC123456789" };
        Context.GiftCards.Add(giftCard);
        await Context.SaveChangesAsync();

        // Act
        giftCard.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedCard = await Context.GiftCards.FirstOrDefaultAsync(gc => gc.CardNumber == "GC123456789");
        Assert.Null(deletedCard);
    }
}
