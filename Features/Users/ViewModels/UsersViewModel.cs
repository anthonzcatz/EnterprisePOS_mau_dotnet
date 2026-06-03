using System.Collections.ObjectModel;
using System.Windows.Input;
using EnterprisePOS.Features.Users.Models;
using EnterprisePOS.Helpers;
using Microsoft.Maui.Controls;

namespace EnterprisePOS.Features.Users.ViewModels;

public class UsersViewModel : BaseViewModel
{
    private readonly List<UserListItem> _allUsers;
    private string _searchQuery = string.Empty;

    public ObservableCollection<UserListItem> Users { get; } = new();

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
            {
                _ = LoadUsersAsync();
            }
        }
    }

    public string TotalUsersText => _allUsers.Count.ToString();
    public string ActiveUsersText => _allUsers.Count(user => user.IsOnline).ToString();
    public string AdminUsersText => _allUsers.Count(user => user.IsAdmin).ToString();
    public string TwoFactorUsersText => _allUsers.Count(user => user.HasTwoFactor).ToString();

    public ICommand RefreshCommand { get; }
    public ICommand AddUserCommand { get; }
    public ICommand ViewDetailsCommand { get; }
    public ICommand ClearSearchCommand { get; }

    public UsersViewModel()
    {
        _allUsers = CreateSeedUsers();

        RefreshCommand = new Command(async () => await LoadUsersAsync());
        AddUserCommand = new Command(async () => await OnAddUserAsync());
        ViewDetailsCommand = new Command<UserListItem>(async user => await OnViewDetailsAsync(user));
        ClearSearchCommand = new Command(() => SearchQuery = string.Empty);

        _ = LoadUsersAsync();
    }

    private Task LoadUsersAsync()
    {
        if (IsBusy)
        {
            return Task.CompletedTask;
        }

        IsBusy = true;
        try
        {
            var filteredUsers = _allUsers
                .Where(user =>
                    string.IsNullOrWhiteSpace(SearchQuery) ||
                    user.FullName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    user.Email.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    user.Role.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    user.Location.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                .OrderBy(user => user.FullName)
                .ToList();

            Users.Clear();
            foreach (var user in filteredUsers)
            {
                Users.Add(user);
            }
        }
        finally
        {
            IsBusy = false;
        }

        return Task.CompletedTask;
    }

    private async Task OnAddUserAsync()
    {
        await Shell.Current.DisplayAlertAsync(
            "Add User",
            "User onboarding flow is not connected yet, but the page is ready for real data and commands.",
            "OK");
    }

    private async Task OnViewDetailsAsync(UserListItem? user)
    {
        if (user is null)
        {
            return;
        }

        var message =
            $"Role: {user.Role}{Environment.NewLine}" +
            $"Email: {user.Email}{Environment.NewLine}" +
            $"Status: {user.Status}{Environment.NewLine}" +
            $"Location: {user.Location}{Environment.NewLine}" +
            $"Last Active: {user.LastActive}";

        await Shell.Current.DisplayAlertAsync(user.FullName, message, "Close");
    }

    private static List<UserListItem> CreateSeedUsers()
    {
        return
        [
            new UserListItem
            {
                FullName = "Angela Reyes",
                Email = "angela.reyes@enterprisepos.local",
                Role = "Operations Admin",
                Status = "Online",
                Location = "HQ - Manila",
                Initials = "AR",
                LastActive = "Now",
                IsOnline = true,
                IsAdmin = true,
                HasTwoFactor = true
            },
            new UserListItem
            {
                FullName = "Brian Cruz",
                Email = "brian.cruz@enterprisepos.local",
                Role = "Inventory Supervisor",
                Status = "In Shift",
                Location = "Warehouse",
                Initials = "BC",
                LastActive = "5 mins ago",
                IsOnline = true,
                IsAdmin = false,
                HasTwoFactor = true
            },
            new UserListItem
            {
                FullName = "Camille Santos",
                Email = "camille.santos@enterprisepos.local",
                Role = "Store Manager",
                Status = "Reviewing",
                Location = "Branch 02",
                Initials = "CS",
                LastActive = "12 mins ago",
                IsOnline = true,
                IsAdmin = true,
                HasTwoFactor = false
            },
            new UserListItem
            {
                FullName = "Daniel Flores",
                Email = "daniel.flores@enterprisepos.local",
                Role = "Cashier",
                Status = "Offline",
                Location = "Branch 01",
                Initials = "DF",
                LastActive = "1 hour ago",
                IsOnline = false,
                IsAdmin = false,
                HasTwoFactor = false
            },
            new UserListItem
            {
                FullName = "Elaine Gomez",
                Email = "elaine.gomez@enterprisepos.local",
                Role = "Finance Reviewer",
                Status = "Online",
                Location = "HQ - Cebu",
                Initials = "EG",
                LastActive = "Now",
                IsOnline = true,
                IsAdmin = false,
                HasTwoFactor = true
            },
            new UserListItem
            {
                FullName = "Francis Lim",
                Email = "francis.lim@enterprisepos.local",
                Role = "Support Analyst",
                Status = "On Call",
                Location = "Remote",
                Initials = "FL",
                LastActive = "27 mins ago",
                IsOnline = true,
                IsAdmin = false,
                HasTwoFactor = true
            }
        ];
    }
}
