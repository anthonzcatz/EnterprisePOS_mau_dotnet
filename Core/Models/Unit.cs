namespace EnterprisePOS.Core.Data.Models;

public class Unit : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public int? BaseUnitId { get; set; }
    public decimal ConversionFactor { get; set; } = 1.0000m;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Product> Products { get; set; } = [];
}
