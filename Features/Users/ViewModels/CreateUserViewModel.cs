using System.Collections.ObjectModel;
using System.Windows.Input;
using EnterprisePOS.Features.Users.Models;
using EnterprisePOS.Helpers;
using Microsoft.Maui.Controls;

namespace EnterprisePOS.Features.Users.ViewModels;

public class CreateUserViewModel : BaseViewModel
{
    private string _fullName = string.Empty;
    private string _username = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _selectedRole = string.Empty;
    private string _selectedBranch = string.Empty;
    private bool _isActive = true;
    private bool _isAdmin;
    private bool _hasTwoFactor;

    public string FullName
    {
        get => _fullName;
        set
        {
            if (SetProperty(ref _fullName, value))
            {
                Validate();
            }
        }
    }

    public string Username
    {
        get => _username;
        set
        {
            if (SetProperty(ref _username, value))
            {
                Validate();
            }
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (SetProperty(ref _email, value))
            {
                Validate();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
            {
                Validate();
            }
        }
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            if (SetProperty(ref _confirmPassword, value))
            {
                Validate();
            }
        }
    }

    public string SelectedRole
    {
        get => _selectedRole;
        set
        {
            if (SetProperty(ref _selectedRole, value))
            {
                Validate();
            }
        }
    }

    public string SelectedBranch
    {
        get => _selectedBranch;
        set
        {
            if (SetProperty(ref _selectedBranch, value))
            {
                Validate();
            }
        }
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
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

    private string _fullNameError = string.Empty;
    public string FullNameError
    {
        get => _fullNameError;
        set => SetProperty(ref _fullNameError, value);
    }

    private string _usernameError = string.Empty;
    public string UsernameError
    {
        get => _usernameError;
        set => SetProperty(ref _usernameError, value);
    }

    private string _emailError = string.Empty;
    public string EmailError
    {
        get => _emailError;
        set => SetProperty(ref _emailError, value);
    }

    private string _passwordError = string.Empty;
    public string PasswordError
    {
        get => _passwordError;
        set => SetProperty(ref _passwordError, value);
    }

    private string _confirmPasswordError = string.Empty;
    public string ConfirmPasswordError
    {
        get => _confirmPasswordError;
        set => SetProperty(ref _confirmPasswordError, value);
    }

    private string _roleError = string.Empty;
    public string RoleError
    {
        get => _roleError;
        set => SetProperty(ref _roleError, value);
    }

    private string _branchError = string.Empty;
    public string BranchError
    {
        get => _branchError;
        set => SetProperty(ref _branchError, value);
    }

    public bool HasFullNameError => !string.IsNullOrEmpty(FullNameError);
    public bool HasUsernameError => !string.IsNullOrEmpty(UsernameError);
    public bool HasEmailError => !string.IsNullOrEmpty(EmailError);
    public bool HasPasswordError => !string.IsNullOrEmpty(PasswordError);
    public bool HasConfirmPasswordError => !string.IsNullOrEmpty(ConfirmPasswordError);
    public bool HasRoleError => !string.IsNullOrEmpty(RoleError);
    public bool HasBranchError => !string.IsNullOrEmpty(BranchError);
    public bool IsNotBusy => !IsBusy;

    public bool HasErrors =>
        !string.IsNullOrEmpty(FullNameError) ||
        !string.IsNullOrEmpty(UsernameError) ||
        !string.IsNullOrEmpty(EmailError) ||
        !string.IsNullOrEmpty(PasswordError) ||
        !string.IsNullOrEmpty(ConfirmPasswordError) ||
        !string.IsNullOrEmpty(RoleError) ||
        !string.IsNullOrEmpty(BranchError);

    public List<string> Roles { get; } = new();
    public List<string> Branches { get; } = new();

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand GeneratePasswordCommand { get; }
    public ICommand SelectRoleCommand { get; }
    public ICommand SelectBranchCommand { get; }

    public CreateUserViewModel()
    {
        SaveCommand = new Command(async () => await OnSaveAsync(), () => !IsBusy);
        CancelCommand = new Command(async () => await OnCancelAsync());
        GeneratePasswordCommand = new Command(() => OnGeneratePassword());
        SelectRoleCommand = new Command(async () => await OnSelectRoleAsync());
        SelectBranchCommand = new Command(async () => await OnSelectBranchAsync());

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(IsBusy))
                OnPropertyChanged(nameof(IsNotBusy));
        };

        LoadSeedData();
    }

    private void LoadSeedData()
    {
        Roles.AddRange(new[] { "Operations Admin", "Inventory Supervisor", "Store Manager", "Cashier", "Finance Reviewer", "Support Analyst" });
        Branches.AddRange(new[] { "HQ - Manila", "HQ - Cebu", "Branch 01", "Branch 02", "Warehouse", "Remote" });
    }

    private async Task OnSelectRoleAsync()
    {
        var role = await Shell.Current.DisplayActionSheet("Select Role", "Cancel", null, Roles.ToArray());
        if (!string.IsNullOrEmpty(role) && role != "Cancel")
        {
            SelectedRole = role;
        }
    }

    private async Task OnSelectBranchAsync()
    {
        var branch = await Shell.Current.DisplayActionSheet("Select Branch", "Cancel", null, Branches.ToArray());
        if (!string.IsNullOrEmpty(branch) && branch != "Cancel")
        {
            SelectedBranch = branch;
        }
    }

    private void Validate()
    {
        FullNameError = string.IsNullOrWhiteSpace(FullName) ? "Full name is required" : string.Empty;
        OnPropertyChanged(nameof(HasFullNameError));
        UsernameError = string.IsNullOrWhiteSpace(Username) ? "Username is required" : string.Empty;
        OnPropertyChanged(nameof(HasUsernameError));
        EmailError = string.IsNullOrWhiteSpace(Email) ? "Email is required" : !Email.Contains('@') ? "Invalid email format" : string.Empty;
        OnPropertyChanged(nameof(HasEmailError));
        PasswordError = string.IsNullOrWhiteSpace(Password) ? "Password is required" : Password.Length < 6 ? "Password must be at least 6 characters" : string.Empty;
        OnPropertyChanged(nameof(HasPasswordError));
        ConfirmPasswordError = ConfirmPassword != Password ? "Passwords do not match" : string.Empty;
        OnPropertyChanged(nameof(HasConfirmPasswordError));
        RoleError = string.IsNullOrWhiteSpace(SelectedRole) ? "Role is required" : string.Empty;
        OnPropertyChanged(nameof(HasRoleError));
        BranchError = string.IsNullOrWhiteSpace(SelectedBranch) ? "Branch is required" : string.Empty;
        OnPropertyChanged(nameof(HasBranchError));
    }

    private bool ValidateAll()
    {
        Validate();
        return !HasErrors;
    }

    private async Task OnSaveAsync()
    {
        if (!ValidateAll())
        {
            await Shell.Current.DisplayAlertAsync("Validation Error", "Please fix the errors before saving.", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            await Task.Delay(500);
            await Shell.Current.DisplayAlertAsync("Success", $"User '{Username}' has been created successfully.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnCancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void OnGeneratePassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
        var random = new Random();
        var password = new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
        Password = password;
        ConfirmPassword = password;
        Validate();
    }
}
