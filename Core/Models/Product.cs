namespace EnterprisePOS.Core.Data.Models;

public class Product : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int CategoryId { get; set; }
    public int? UnitId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsTaxable { get; set; } = true;
    public string? ImageUrl { get; set; }

    // Navigation properties
    public ProductCategory? Category { get; set; }
    public Unit? Unit { get; set; }
    public ICollection<ProductVariant> ProductVariants { get; set; } = [];
}
