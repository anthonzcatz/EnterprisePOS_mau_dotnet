namespace EnterprisePOS.Core.Data.Models;

public class Customer : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public int LoyaltyPoints { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Sale> Sales { get; set; } = [];
}
