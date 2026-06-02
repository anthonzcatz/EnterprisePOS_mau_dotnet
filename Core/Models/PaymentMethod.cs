namespace EnterprisePOS.Core.Data.Models;

public class PaymentMethod : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<SalePayment> SalePayments { get; set; } = [];
}
