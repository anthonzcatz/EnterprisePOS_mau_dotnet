namespace EnterprisePOS.Core.Data.Models;

public class Sale : BaseEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public int? BranchId { get; set; }
    public int? TerminalId { get; set; }
    public int? CustomerId { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Branch? Branch { get; set; }
    public Terminal? Terminal { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; } = [];
    public ICollection<SalePayment> SalePayments { get; set; } = [];
}
