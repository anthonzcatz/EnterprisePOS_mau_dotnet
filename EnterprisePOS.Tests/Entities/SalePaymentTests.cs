using EnterprisePOS.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterprisePOS.Tests.Entities;

public class SalePaymentTests : TestBase
{
    [Fact]
    public async Task CanCreateSalePayment()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var terminal = new PosTerminal { Name = "Terminal 1", Code = "T1", Branch = branch };
        var paymentMethod = new PaymentMethod { Name = "Cash", Code = "CASH" };
        var sale = new Sale { Branch = branch, Terminal = terminal, SaleNumber = "SALE001", TotalAmount = 100.00m };
        
        var salePayment = new SalePayment
        {
            Sale = sale,
            PaymentMethod = paymentMethod,
            Amount = 100.00m,
            ReferenceNumber = "REF001"
        };

        // Act
        Context.Branches.Add(branch);
        Context.PosTerminals.Add(terminal);
        Context.PaymentMethods.Add(paymentMethod);
        Context.Sales.Add(sale);
        Context.SalePayments.Add(salePayment);
        await Context.SaveChangesAsync();

        // Assert
        var savedPayment = await Context.SalePayments
            .Include(sp => sp.PaymentMethod)
            .FirstOrDefaultAsync(sp => sp.ReferenceNumber == "REF001");
        
        Assert.NotNull(savedPayment);
        Assert.Equal(100.00m, savedPayment.Amount);
        Assert.Equal("Cash", savedPayment.PaymentMethod.Name);
    }

    [Fact]
    public async Task CanUpdateSalePayment()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var terminal = new PosTerminal { Name = "Terminal 1", Code = "T1", Branch = branch };
        var paymentMethod = new PaymentMethod { Name = "Cash", Code = "CASH" };
        var sale = new Sale { Branch = branch, Terminal = terminal, SaleNumber = "SALE001", TotalAmount = 100.00m };
        var payment = new SalePayment { Sale = sale, PaymentMethod = paymentMethod, Amount = 100.00m };
        
        Context.Branches.Add(branch);
        Context.PosTerminals.Add(terminal);
        Context.PaymentMethods.Add(paymentMethod);
        Context.Sales.Add(sale);
        Context.SalePayments.Add(payment);
        await Context.SaveChangesAsync();

        // Act
        payment.ReferenceNumber = "REF002";
        await Context.SaveChangesAsync();

        // Assert
        var updatedPayment = await Context.SalePayments.FirstOrDefaultAsync(sp => sp.SaleId == sale.Id);
        Assert.Equal("REF002", updatedPayment?.ReferenceNumber);
    }

    [Fact]
    public async Task CanDeleteSalePayment_SoftDelete()
    {
        // Arrange
        var branch = new Branch { Name = "Main", Code = "MAIN" };
        var terminal = new PosTerminal { Name = "Terminal 1", Code = "T1", Branch = branch };
        var paymentMethod = new PaymentMethod { Name = "Cash", Code = "CASH" };
        var sale = new Sale { Branch = branch, Terminal = terminal, SaleNumber = "SALE001", TotalAmount = 100.00m };
        var payment = new SalePayment { Sale = sale, PaymentMethod = paymentMethod, Amount = 100.00m };
        
        Context.Branches.Add(branch);
        Context.PosTerminals.Add(terminal);
        Context.PaymentMethods.Add(paymentMethod);
        Context.Sales.Add(sale);
        Context.SalePayments.Add(payment);
        await Context.SaveChangesAsync();

        // Act
        payment.IsDeleted = true;
        await Context.SaveChangesAsync();

        // Assert
        var deletedPayment = await Context.SalePayments.FirstOrDefaultAsync(sp => sp.SaleId == sale.Id);
        Assert.Null(deletedPayment);
    }
}
