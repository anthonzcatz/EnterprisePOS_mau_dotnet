namespace EnterprisePOS.Features.UserManagement.Models;

public class UserModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Cashier";
    public string Branch { get; set; } = "HQ - Manila";
    public bool IsActive { get; set; } = true;
    public bool IsAdmin { get; set; } = false;
    public bool HasTwoFactor { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Status => IsActive ? "Active" : "Inactive";
    public string Initials
    {
        get
        {
            var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0
                ? "U"
                : string.Concat(parts.Take(2).Select(part => char.ToUpper(part[0])));
        }
    }

    public string CreatedAtText => CreatedAt.ToLocalTime().ToString("MMM d, yyyy");
}
