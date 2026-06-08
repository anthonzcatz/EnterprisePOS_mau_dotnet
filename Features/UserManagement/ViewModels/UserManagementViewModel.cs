using System.Collections.ObjectModel;
using System.Windows.Input;
using EnterprisePOS.Features.UserManagement.Models;
using EnterprisePOS.Helpers;
using EnterprisePOS.Navigation;
using EnterprisePOS.Core.Data.Repositories;
using EnterprisePOS.Core.Data.Models;
using Microsoft.Maui.Controls;

namespace EnterprisePOS.Features.UserManagement.ViewModels;

public class UserManagementViewModel : BaseViewModel
{
    private readonly UserRepository _userRepository;
    private string _searchQuery = string.Empty;
    private bool _isCardView = true;

    public ObservableCollection<UserModel> Users { get; } = new();

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
            {
                FilterUsers();
            }
        }
    }

    public ICommand AddUserCommand { get; }
    public ICommand DeleteUserCommand { get; }
    public ICommand EditUserCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ShowCardViewCommand { get; }
    public ICommand ShowListViewCommand { get; }

    private List<UserModel> _allUsers = new();

    public string TotalUsersText => _allUsers.Count.ToString();
    public string ActiveUsersText => _allUsers.Count(user => user.IsActive).ToString();
    public string AdminUsersText => _allUsers.Count(user => user.IsAdmin).ToString();
    public string TwoFactorUsersText => _allUsers.Count(user => user.HasTwoFactor).ToString();

    public bool IsCardView
    {
        get => _isCardView;
        set
        {
            if (SetProperty(ref _isCardView, value))
            {
                OnPropertyChanged(nameof(IsListView));
            }
        }
    }

    public bool IsListView => !IsCardView;

    public UserManagementViewModel(UserRepository userRepository)
    {
        _userRepository = userRepository;
        AddUserCommand = new Command(async () => await OnAddUserAsync());
        DeleteUserCommand = new Command<UserModel>(async (user) => await DeleteUserAsync(user));
        EditUserCommand = new Command<UserModel>(async (user) => await EditUserAsync(user));
        RefreshCommand = new Command(async () => await LoadUsersAsync());
        ShowCardViewCommand = new Command(() => IsCardView = true);
        ShowListViewCommand = new Command(() => IsCardView = false);

        _ = LoadUsersAsync();
    }

    private async Task OnAddUserAsync()
    {
        try
        {
            await Shell.Current.GoToAsync(Routes.UserMgmtCreate);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Navigation Error", ex.Message, "OK");
        }
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            IsBusy = true;
            var users = await _userRepository.GetAllAsync();
            
            _allUsers = users.Select(u => new UserModel
            {
                Id = u.Id,
                FullName = u.FullName,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                Branch = u.Branch,
                IsActive = u.IsActive,
                IsAdmin = u.IsAdmin,
                HasTwoFactor = u.HasTwoFactor,
                CreatedAt = u.CreatedAt
            }).ToList();

            OnPropertyChanged(nameof(TotalUsersText));
            OnPropertyChanged(nameof(ActiveUsersText));
            OnPropertyChanged(nameof(AdminUsersText));
            OnPropertyChanged(nameof(TwoFactorUsersText));
            
            FilterUsers();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Failed to load users: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void FilterUsers()
    {
        Users.Clear();
        var filtered = string.IsNullOrWhiteSpace(SearchQuery)
            ? _allUsers
            : _allUsers.Where(u => u.FullName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                     u.Username.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                     u.Email.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                     u.Role.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                     u.Branch.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

        foreach (var user in filtered)
        {
            Users.Add(user);
        }
    }

    private async Task DeleteUserAsync(UserModel? user)
    {
        if (user == null) return;

        var confirm = await Shell.Current.DisplayAlertAsync("Confirm", $"Delete user '{user.Username}'?", "Yes", "No");
        if (confirm)
        {
            try
            {
                await _userRepository.DeleteAsync(user.Id);
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to delete user: {ex.Message}", "OK");
            }
        }
    }

    private async Task EditUserAsync(UserModel? user)
    {
        if (user == null) return;

        try
        {
            var entity = await _userRepository.GetByIdAsync(user.Id);
            if (entity == null)
            {
                await Shell.Current.DisplayAlertAsync("User Not Found", "This user record no longer exists.", "OK");
                await LoadUsersAsync();
                return;
            }

            var fullName = await Shell.Current.DisplayPromptAsync("Update User", "Full name", "Save", "Cancel", initialValue: entity.FullName);
            if (string.IsNullOrWhiteSpace(fullName)) return;

            var role = await Shell.Current.DisplayPromptAsync("Update Role", "Role", "Save", "Cancel", initialValue: entity.Role);
            if (string.IsNullOrWhiteSpace(role)) return;

            var branch = await Shell.Current.DisplayPromptAsync("Update Branch", "Branch", "Save", "Cancel", initialValue: entity.Branch);
            if (string.IsNullOrWhiteSpace(branch)) return;

            entity.FullName = fullName.Trim();
            entity.Role = role.Trim();
            entity.Branch = branch.Trim();

            await _userRepository.UpdateAsync(entity);
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Update Error", $"Failed to update user: {ex.Message}", "OK");
        }
    }
}
