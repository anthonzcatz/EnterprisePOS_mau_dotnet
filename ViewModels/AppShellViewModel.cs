using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EnterprisePOS.Features.POS.Models;
using EnterprisePOS.Helpers;
using Microsoft.Maui.Controls;

namespace EnterprisePOS.ViewModels;

public sealed class AppShellViewModel : INotifyPropertyChanged
{
	private bool isSidebarCollapsed = true;
	private bool isMobileDrawerOpen;
	private double availableWidth = 1200;
	private const double SidebarExpandedWidth = 240;
	private const double SidebarCollapsedWidth = 76;

	public event PropertyChangedEventHandler? PropertyChanged;

	public AppShellViewModel()
	{
		ToggleSidebarCommand = new Command(ToggleSidebar);
		NavigateCommand = new Command<PosNavItem>(Navigate);
		OpenMobileDrawerCommand = new Command(() => IsMobileDrawerOpen = true);
		CloseMobileDrawerCommand = new Command(() => IsMobileDrawerOpen = false);
		LoadNavItems();
	}

	public ObservableCollection<PosNavItem> NavItems { get; } = [];

	public bool IsSidebarCollapsed
	{
		get => isSidebarCollapsed;
		set
		{
			if (isSidebarCollapsed == value) return;
			isSidebarCollapsed = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(SidebarGridLength));
		}
	}

	public bool IsMobileDrawerOpen
	{
		get => isMobileDrawerOpen;
		set
		{
			if (isMobileDrawerOpen == value) return;
			isMobileDrawerOpen = value;
			OnPropertyChanged();
		}
	}

	public bool ShowSidebar => availableWidth >= LayoutBreakpoints.SidebarVisibleMin;

	public bool ShowMobileNavbar => !ShowSidebar;

	public GridLength SidebarGridLength =>
		ShowSidebar ? new(IsSidebarCollapsed ? SidebarCollapsedWidth : SidebarExpandedWidth) : new(0);

	public void UpdateAvailableWidth(double width)
	{
		if (width <= 0 || Math.Abs(availableWidth - width) < 1) return;
		availableWidth = width;
		OnPropertyChanged(nameof(ShowSidebar));
		OnPropertyChanged(nameof(ShowMobileNavbar));
		OnPropertyChanged(nameof(SidebarGridLength));
		if (ShowSidebar) IsMobileDrawerOpen = false;
	}

	public Command ToggleSidebarCommand { get; }
	public Command<PosNavItem> NavigateCommand { get; }
	public Command OpenMobileDrawerCommand { get; }
	public Command CloseMobileDrawerCommand { get; }

	public void SetActiveRoute(string routeKey)
	{
		foreach (var nav in NavItems)
		{
			SetActiveRouteRecursive(nav, routeKey);
		}
	}

	private void ToggleSidebar() => IsSidebarCollapsed = !IsSidebarCollapsed;

	private async void Navigate(PosNavItem? item)
	{
		if (item is null) return;

		if (item.HasChildren)
		{
			ToggleSubmenu(item);
			return;
		}

		var route = item.Route;
		if (string.IsNullOrWhiteSpace(route)) return;

		SetActiveRoute(item.Key);
		IsMobileDrawerOpen = false;

		try
		{
			await Shell.Current.GoToAsync(route, false);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
		}
	}

	private void LoadNavItems()
	{
		NavItems.Add(new PosNavItem { Key = "home",            Title = "Home",          Glyph = "⌂", Route = "//dashboard/dashboard-main" });
		NavItems.Add(new PosNavItem { Key = "pos",             Title = "Point of Sale", Glyph = "⊞", Route = "//pos/pos-main" });
		NavItems.Add(new PosNavItem { Key = "orders",          Title = "Orders",        Glyph = "🧾", Route = "//sales/sales-main" });
		NavItems.Add(new PosNavItem { Key = "customers",       Title = "Guests",        Glyph = "👤", Route = "//customers/customers-main" });

		var catalog = new PosNavItem { Key = "catalog", Title = "Products", Glyph = "📦" };
		catalog.Children.Add(new PosNavItem { Key = "products", Title = "Product List", Glyph = "•", Route = "//products", IsChild = true });
		catalog.Children.Add(new PosNavItem { Key = "categories", Title = "Categories", Glyph = "•", Route = "//categories", IsChild = true });
		catalog.Children.Add(new PosNavItem { Key = "units", Title = "Units", Glyph = "•", Route = "//units", IsChild = true });
		NavItems.Add(catalog);

		NavItems.Add(new PosNavItem { Key = "inventory",       Title = "Inventory",     Glyph = "🗃", Route = "//inventory/inventory-main" });
		NavItems.Add(new PosNavItem { Key = "reports",         Title = "Reports",       Glyph = "📊", Route = "//reports/reports-main" });
		NavItems.Add(new PosNavItem { Key = "user-management", Title = "Users",         Glyph = "👤", Route = "//user-management/user-management-main" });
		NavItems.Add(new PosNavItem { Key = "settings",        Title = "Settings",      Glyph = "⚙", Route = "//settings/settings-main" });
	}

	private void ToggleSubmenu(PosNavItem selected)
	{
		foreach (var nav in NavItems.Where(n => n.HasChildren))
		{
			nav.IsExpanded = nav == selected && !nav.IsExpanded;
		}
	}

	private bool SetActiveRouteRecursive(PosNavItem item, string routeKey)
	{
		var isActive = item.Key == routeKey;
		var hasActiveChild = false;

		foreach (var child in item.Children)
		{
			if (SetActiveRouteRecursive(child, routeKey))
			{
				hasActiveChild = true;
				isActive = true;
			}
		}

		if (item.HasChildren)
		{
			item.IsExpanded = hasActiveChild;
		}

		item.IsActive = isActive;
		return isActive;
	}

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
