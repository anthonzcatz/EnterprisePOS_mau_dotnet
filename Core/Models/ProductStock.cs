namespace EnterprisePOS.Core.Data.Models;

public class ProductStock : BaseEntity
{
    public int ProductId { get; set; }
    public decimal BaseQuantity { get; set; }
    public decimal DisplayQuantity { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal MaxStock { get; set; }

    // Navigation properties
    public Product Product { get; set; } = null!;
}
