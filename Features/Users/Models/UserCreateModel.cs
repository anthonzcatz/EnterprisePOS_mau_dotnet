namespace EnterprisePOS.Features.Users.Models;

public class UserCreateModel
{
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string SelectedRole { get; set; } = string.Empty;
    public string SelectedBranch { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsAdmin { get; set; }
    public bool HasTwoFactor { get; set; }
    public string? ImageUrl { get; set; }
}
