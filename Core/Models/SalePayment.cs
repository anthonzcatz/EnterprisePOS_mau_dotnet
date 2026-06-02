namespace EnterprisePOS.Core.Data.Models;

public class SalePayment : BaseEntity
{
    public int SaleId { get; set; }
    public int PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }

    // Navigation properties
    public Sale Sale { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
}
