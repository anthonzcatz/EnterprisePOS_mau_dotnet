namespace EnterprisePOS.Core.Data.Models;

public class Branch : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Sale> Sales { get; set; } = [];
    public ICollection<PosTerminal> Terminals { get; set; } = [];
}
