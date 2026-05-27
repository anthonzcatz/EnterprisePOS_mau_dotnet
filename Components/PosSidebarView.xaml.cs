using System.Collections;
using System.Windows.Input;
using EnterprisePOS.Models;

namespace EnterprisePOS.Components;

public partial class PosSidebarView : ContentView
{
	public static readonly BindableProperty NavItemsProperty =
		BindableProperty.Create(nameof(NavItems), typeof(IEnumerable), typeof(PosSidebarView));

	public static readonly BindableProperty ToggleCommandProperty =
		BindableProperty.Create(nameof(ToggleCommand), typeof(ICommand), typeof(PosSidebarView));

	public static readonly BindableProperty IsCollapsedProperty =
		BindableProperty.Create(
			nameof(IsCollapsed),
			typeof(bool),
			typeof(PosSidebarView),
			false,
			BindingMode.TwoWay,
			propertyChanged: OnCollapsedChanged);

	public static readonly BindableProperty NavigateCommandProperty =
		BindableProperty.Create(nameof(NavigateCommand), typeof(ICommand), typeof(PosSidebarView));

	public PosSidebarView()
	{
		InitializeComponent();
		UpdateCollapseState();
	}

	public IEnumerable? NavItems
	{
		get => (IEnumerable?)GetValue(NavItemsProperty);
		set => SetValue(NavItemsProperty, value);
	}

	public bool IsCollapsed
	{
		get => (bool)GetValue(IsCollapsedProperty);
		set => SetValue(IsCollapsedProperty, value);
	}

	public ICommand? ToggleCommand
	{
		get => (ICommand?)GetValue(ToggleCommandProperty);
		set => SetValue(ToggleCommandProperty, value);
	}

	public ICommand? NavigateCommand
	{
		get => (ICommand?)GetValue(NavigateCommandProperty);
		set => SetValue(NavigateCommandProperty, value);
	}

	public bool ShowLabels => !IsCollapsed;

	public string ToggleIcon => IsCollapsed ? "›" : "‹";

	public LayoutOptions HeaderHorizontalOptions =>
		IsCollapsed ? LayoutOptions.Center : LayoutOptions.Start;

	public LayoutOptions ItemHorizontalOptions =>
		IsCollapsed ? LayoutOptions.Center : LayoutOptions.Start;

	public LayoutOptions ItemRowHorizontalOptions =>
		IsCollapsed ? LayoutOptions.Center : LayoutOptions.Fill;

	public LayoutOptions ActiveBackgroundHorizontalOptions =>
		IsCollapsed ? LayoutOptions.Center : LayoutOptions.Fill;

	public double IconSpacing => IsCollapsed ? 0 : 12;

	public double ItemRowWidth => IsCollapsed ? 60 : -1;

	public double ActivePillWidth => IsCollapsed ? 48 : -1;

	public Thickness ActivePillPadding => IsCollapsed ? new Thickness(0) : new Thickness(6, 10);

	private static void OnCollapsedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is PosSidebarView sidebar)
		{
			sidebar.UpdateCollapseState();
		}
	}

	private void UpdateCollapseState()
	{
		OnPropertyChanged(nameof(ShowLabels));
		OnPropertyChanged(nameof(ToggleIcon));
		OnPropertyChanged(nameof(HeaderHorizontalOptions));
		OnPropertyChanged(nameof(ItemHorizontalOptions));
		OnPropertyChanged(nameof(ItemRowHorizontalOptions));
		OnPropertyChanged(nameof(ActiveBackgroundHorizontalOptions));
		OnPropertyChanged(nameof(ItemRowWidth));
		OnPropertyChanged(nameof(IconSpacing));
		OnPropertyChanged(nameof(ActivePillWidth));
		OnPropertyChanged(nameof(ActivePillPadding));
	}
}
