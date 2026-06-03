namespace EnterprisePOS.Features.Users.Models;

public class UserListItem
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string Initials { get; init; } = string.Empty;
    public string LastActive { get; init; } = string.Empty;
    public bool IsOnline { get; init; }
    public bool IsAdmin { get; init; }
    public bool HasTwoFactor { get; init; }
}
