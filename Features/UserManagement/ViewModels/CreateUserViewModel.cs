using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using EnterprisePOS.Features.UserManagement.Models;
using EnterprisePOS.Helpers;
using EnterprisePOS.Core.Data.Repositories;
using EnterprisePOS.Core.Data.Models;
using Microsoft.Maui.Controls;

namespace EnterprisePOS.Features.UserManagement.ViewModels;

public class CreateUserViewModel : BaseViewModel
{
    private readonly UserRepository _userRepository;
    private string _fullName = string.Empty;
    private string _username = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _role = "Cashier";
    private string _branch = "HQ - Manila";
    private bool _isAdmin;
    private bool _hasTwoFactor;

    // Validation error properties
    private string _fullNameError = string.Empty;
    private string _usernameError = string.Empty;
    private string _emailError = string.Empty;
    private string _passwordError = string.Empty;
    private string _roleError = string.Empty;
    private string _branchError = string.Empty;
    private string _formError = string.Empty;

    private void SetError(ref string field, string value, string propertyName)
    {
        field = value;
        OnPropertyChanged(propertyName);
    }

    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }

    public string Branch
    {
        get => _branch;
        set => SetProperty(ref _branch, value);
    }

    public bool IsAdmin
    {
        get => _isAdmin;
        set => SetProperty(ref _isAdmin, value);
    }

    public bool HasTwoFactor
    {
        get => _hasTwoFactor;
        set => SetProperty(ref _hasTwoFactor, value);
    }

    // Validation error properties
    public string FullNameError
    {
        get => _fullNameError;
        set => SetError(ref _fullNameError, value, nameof(FullNameError));
    }

    public string UsernameError
    {
        get => _usernameError;
        set => SetError(ref _usernameError, value, nameof(UsernameError));
    }

    public string EmailError
    {
        get => _emailError;
        set => SetError(ref _emailError, value, nameof(EmailError));
    }

    public string PasswordError
    {
        get => _passwordError;
        set => SetError(ref _passwordError, value, nameof(PasswordError));
    }

    public string RoleError
    {
        get => _roleError;
        set => SetError(ref _roleError, value, nameof(RoleError));
    }

    public string BranchError
    {
        get => _branchError;
        set => SetError(ref _branchError, value, nameof(BranchError));
    }

    public string FormError
    {
        get => _formError;
        set => SetError(ref _formError, value, nameof(FormError));
    }

    public ObservableCollection<string> AvailableRoles { get; } = new ObservableCollection<string> { "Admin", "Manager", "Cashier", "Inventory", "Viewer" };
    public ObservableCollection<string> AvailableBranches { get; } = new ObservableCollection<string> { "HQ - Manila", "HQ - Cebu", "Branch 01", "Branch 02", "Warehouse" };

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand GeneratePasswordCommand { get; }

    public CreateUserViewModel(UserRepository userRepository)
    {
        _userRepository = userRepository;
        System.Diagnostics.Debug.WriteLine("[CreateUserViewModel] Constructor called");
        System.Diagnostics.Debug.WriteLine($"[CreateUserViewModel] AvailableRoles count: {AvailableRoles.Count}");
        System.Diagnostics.Debug.WriteLine($"[CreateUserViewModel] AvailableBranches count: {AvailableBranches.Count}");
        
        SaveCommand = new Command(async () => {
            System.Diagnostics.Debug.WriteLine("[CreateUser] SaveCommand EXECUTED!");
            await SaveAsync();
        });
        CancelCommand = new Command(async () => await CancelAsync());
        GeneratePasswordCommand = new Command(GeneratePassword);
        
        System.Diagnostics.Debug.WriteLine("[CreateUserViewModel] Commands initialized");
        System.Diagnostics.Debug.WriteLine($"[CreateUserViewModel] SaveCommand: {SaveCommand}");
    }

    private async Task SaveAsync()
    {
        if (IsBusy)
        {
            return;
        }

        var validationErrors = ValidateFields();
        
        if (validationErrors.Count > 0)
        {
            FormError = "Please complete the required fields before saving.";
            return;
        }

        IsBusy = true;
        try
        {
            var normalizedUsername = Username.Trim().ToLower();
            var normalizedEmail = Email.Trim().ToLower();

            // Check if username already exists
            if (await _userRepository.UsernameExistsAsync(normalizedUsername))
            {
                UsernameError = "Username already exists. Please choose a different username.";
                FormError = "This username is already in use.";
                return;
            }

            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(normalizedEmail))
            {
                EmailError = "Email already exists. Please use a different email address.";
                FormError = "This email address is already in use.";
                return;
            }

            // Create new user
            var user = new User
            {
                FullName = FullName.Trim(),
                Username = normalizedUsername,
                Email = normalizedEmail,
                PasswordHash = HashPassword(Password),
                Role = Role,
                Branch = Branch,
                IsAdmin = IsAdmin,
                HasTwoFactor = HasTwoFactor,
                IsActive = true
            };

            // Save to local database (offline)
            await _userRepository.AddAsync(user);
            
            // Navigate back to user list
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            FormError = $"Failed to create user: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private Page? GetCurrentPage()
    {
        try
        {
            var app = Application.Current;
            
            if (app?.Windows != null && app.Windows.Count > 0)
            {
                return app.Windows[0].Page;
            }
            
            // Fallback to Shell.Current
            return Shell.Current.CurrentPage;
        }
        catch
        {
            return null;
        }
    }

    private List<string> ValidateFields()
    {
        var errors = new List<string>();

        System.Diagnostics.Debug.WriteLine("[CreateUser] ValidateFields started");

        // Clear previous errors
        FullNameError = string.Empty;
        UsernameError = string.Empty;
        EmailError = string.Empty;
        PasswordError = string.Empty;
        RoleError = string.Empty;
        BranchError = string.Empty;
        FormError = string.Empty;

        System.Diagnostics.Debug.WriteLine("[CreateUser] Cleared all error properties");

        // Full Name validation
        if (string.IsNullOrWhiteSpace(FullName))
        {
            errors.Add("Full Name is required.");
            FullNameError = "Full Name is required.";
            System.Diagnostics.Debug.WriteLine("[CreateUser] Set FullNameError: " + FullNameError);
        }
        else if (FullName.Trim().Length < 2)
        {
            errors.Add("Full Name must be at least 2 characters long.");
            FullNameError = "Full Name must be at least 2 characters long.";
            System.Diagnostics.Debug.WriteLine("[CreateUser] Set FullNameError: " + FullNameError);
        }

        // Username validation
        if (string.IsNullOrWhiteSpace(Username))
        {
            errors.Add("Username is required.");
            UsernameError = "Username is required.";
            System.Diagnostics.Debug.WriteLine("[CreateUser] Set UsernameError: " + UsernameError);
        }
        else
        {
            var trimmedUsername = Username.Trim();
            if (trimmedUsername.Length < 3)
            {
                errors.Add("Username must be at least 3 characters long.");
                UsernameError = "Username must be at least 3 characters long.";
                System.Diagnostics.Debug.WriteLine("[CreateUser] Set UsernameError: " + UsernameError);
            }
            if (trimmedUsername.Contains(' '))
            {
                errors.Add("Username cannot contain spaces.");
                UsernameError = "Username cannot contain spaces.";
                System.Diagnostics.Debug.WriteLine("[CreateUser] Set UsernameError: " + UsernameError);
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(trimmedUsername, @"^[a-zA-Z0-9_]+$"))
            {
                errors.Add("Username can only contain letters, numbers, and underscores.");
                UsernameError = "Username can only contain letters, numbers, and underscores.";
                System.Diagnostics.Debug.WriteLine("[CreateUser] Set UsernameError: " + UsernameError);
            }
        }

        // Email validation
        if (string.IsNullOrWhiteSpace(Email))
        {
            errors.Add("Email is required.");
            EmailError = "Email is required.";
            System.Diagnostics.Debug.WriteLine("[CreateUser] Set EmailError: " + EmailError);
        }
        else
        {
            var trimmedEmail = Email.Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(trimmedEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errors.Add("Please enter a valid email address.");
                EmailError = "Please enter a valid email address.";
                System.Diagnostics.Debug.WriteLine("[CreateUser] Set EmailError: " + EmailError);
            }
        }

        // Password validation
        if (string.IsNullOrWhiteSpace(Password))
        {
            errors.Add("Password is required.");
            PasswordError = "Password is required.";
            System.Diagnostics.Debug.WriteLine("[CreateUser] Set PasswordError: " + PasswordError);
        }
        else
        {
            if (Password.Length < 8)
            {
                errors.Add("Password must be at least 8 characters long.");
                PasswordError = "Password must be at least 8 characters long.";
                System.Diagnostics.Debug.WriteLine("[CreateUser] Set PasswordError: " + PasswordError);
            }
            if (!Password.Any(char.IsUpper))
            {
                errors.Add("Password must contain at least one uppercase letter.");
                PasswordError = "Password must contain at least one uppercase letter.";
                System.Diagnostics.Debug.WriteLine("[CreateUser] Set PasswordError: " + PasswordError);
            }
            if (!Password.Any(char.IsLower))
            {
                errors.Add("Password must contain at least one lowercase letter.");
                PasswordError = "Password must contain at least one lowercase letter.";
                System.Diagnostics.Debug.WriteLine("[CreateUser] Set PasswordError: " + PasswordError);
            }
            if (!Password.Any(char.IsDigit))
            {
                errors.Add("Password must contain at least one number.");
                PasswordError = "Password must contain at least one number.";
                System.Diagnostics.Debug.WriteLine("[CreateUser] Set PasswordError: " + PasswordError);
            }
        }

        // Role validation
        if (string.IsNullOrWhiteSpace(Role))
        {
            errors.Add("Please select a Role.");
            RoleError = "Please select a Role.";
            System.Diagnostics.Debug.WriteLine("[CreateUser] Set RoleError: " + RoleError);
        }

        // Branch validation
        if (string.IsNullOrWhiteSpace(Branch))
        {
            errors.Add("Please select a Branch.");
            BranchError = "Please select a Branch.";
            System.Diagnostics.Debug.WriteLine("[CreateUser] Set BranchError: " + BranchError);
        }

        System.Diagnostics.Debug.WriteLine("[CreateUser] Validation complete. Final error properties:");
        System.Diagnostics.Debug.WriteLine("[CreateUser] FullNameError: '" + FullNameError + "'");
        System.Diagnostics.Debug.WriteLine("[CreateUser] UsernameError: '" + UsernameError + "'");
        System.Diagnostics.Debug.WriteLine("[CreateUser] EmailError: '" + EmailError + "'");
        System.Diagnostics.Debug.WriteLine("[CreateUser] PasswordError: '" + PasswordError + "'");
        System.Diagnostics.Debug.WriteLine("[CreateUser] RoleError: '" + RoleError + "'");
        System.Diagnostics.Debug.WriteLine("[CreateUser] BranchError: '" + BranchError + "'");

        return errors;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var builder = new StringBuilder();
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }

    private async Task CancelAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[CreateUser] Cancel clicked, navigating back...");
            await Shell.Current.GoToAsync("..");
            System.Diagnostics.Debug.WriteLine("[CreateUser] Navigation back succeeded");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[CreateUser] Cancel error: {ex}");
            var page = GetCurrentPage();
            if (page != null)
            {
                await page.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
    }

    private void GeneratePassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
        var random = new Random();
        Password = new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
