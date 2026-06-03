namespace EnterprisePOS.Core.Data.Models;

public class PosTerminal : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public string? SerialNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Branch Branch { get; set; } = null!;
    public ICollection<Sale> Sales { get; set; } = [];
}
