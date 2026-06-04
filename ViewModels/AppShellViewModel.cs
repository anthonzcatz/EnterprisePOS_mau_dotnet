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
	private double availableWidth = 1200;
	private const double SidebarExpandedWidth = 240;
	private const double SidebarCollapsedWidth = 76;

	public event PropertyChangedEventHandler? PropertyChanged;

	public AppShellViewModel()
	{
		ToggleSidebarCommand = new Command(ToggleSidebar);
		NavigateCommand = new Command<PosNavItem>(Navigate);
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

	public bool ShowSidebar => availableWidth >= LayoutBreakpoints.SidebarVisibleMin;

	public GridLength SidebarGridLength =>
		ShowSidebar ? new(IsSidebarCollapsed ? SidebarCollapsedWidth : SidebarExpandedWidth) : new(0);

	public void UpdateAvailableWidth(double width)
	{
		if (Math.Abs(availableWidth - width) < 1) return;
		availableWidth = width;
		OnPropertyChanged(nameof(ShowSidebar));
		OnPropertyChanged(nameof(SidebarGridLength));
	}

	public Command ToggleSidebarCommand { get; }
	public Command<PosNavItem> NavigateCommand { get; }

	public void SetActiveRoute(string routeKey)
	{
		foreach (var nav in NavItems)
			nav.IsActive = nav.Key == routeKey;
	}

	private void ToggleSidebar() => IsSidebarCollapsed = !IsSidebarCollapsed;

	private async void Navigate(PosNavItem? item)
	{
		if (item is null) return;

		var route = item.Key switch
		{
			"home"       => "//dashboard/dashboard-main",
			"pos"        => "//pos/pos-main",
			"orders"     => "//sales/sales-main",
			"kitchen"    => "//sales/sales-main",
			"customers"  => "//customers/customers-main",
			"reports"    => "//reports/reports-main",
			_            => null
		};

		if (route is null) return;

		SetActiveRoute(item.Key);

		try
		{
			await Shell.Current.GoToAsync(route);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
		}
	}

	private void LoadNavItems()
	{
		NavItems.Add(new PosNavItem { Key = "home",       Title = "Home",       Glyph = "⌂" });
		NavItems.Add(new PosNavItem { Key = "pos",        Title = "Register",   Glyph = "⊞", IsActive = true });
		NavItems.Add(new PosNavItem { Key = "orders",     Title = "Orders",     Glyph = "🧾" });
		NavItems.Add(new PosNavItem { Key = "kitchen",    Title = "Kitchen",    Glyph = "🍳" });
		NavItems.Add(new PosNavItem { Key = "customers",  Title = "Guests",     Glyph = "�" });
		NavItems.Add(new PosNavItem { Key = "reports",    Title = "Reports",    Glyph = "📊" });
	}

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
