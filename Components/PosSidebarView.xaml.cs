using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using EnterprisePOS.Features.POS.Models;

namespace EnterprisePOS.Components;

public partial class PosSidebarView : ContentView
{
	private readonly HashSet<PosNavItem> subscribedItems = [];
	private PosNavItem? expandedParentItem;
	private ObservableCollection<PosNavItem>? observedNavItems;

	public static readonly BindableProperty NavItemsProperty =
		BindableProperty.Create(
			nameof(NavItems),
			typeof(IEnumerable),
			typeof(PosSidebarView),
			propertyChanged: OnNavItemsChanged);

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

	public static readonly BindableProperty PopupWithinBoundsProperty =
		BindableProperty.Create(
			nameof(PopupWithinBounds),
			typeof(bool),
			typeof(PosSidebarView),
			false,
			propertyChanged: (bindable, _, _) =>
			{
				if (bindable is PosSidebarView sidebar)
				{
					sidebar.OnPropertyChanged(nameof(PopupTranslationX));
					sidebar.OnPropertyChanged(nameof(PopupWidth));
					sidebar.OnPropertyChanged(nameof(PopupMargin));
				}
			});

	public PosSidebarView()
	{
		InitializeComponent();
		SizeChanged += OnSidebarSizeChanged;
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

	public bool PopupWithinBounds
	{
		get => (bool)GetValue(PopupWithinBoundsProperty);
		set => SetValue(PopupWithinBoundsProperty, value);
	}

	public bool ShowLabels => !IsCollapsed;
	public string ToggleIcon => IsCollapsed ? "›" : "‹";
	public LayoutOptions HeaderHorizontalOptions => IsCollapsed ? LayoutOptions.Center : LayoutOptions.Start;
	public Thickness SidebarPadding => IsCollapsed ? new Thickness(8, 16, 8, 12) : new Thickness(12, 16, 8, 12);
	public Thickness NavListPadding => IsCollapsed ? new Thickness(0) : new Thickness(12, 0);

	public bool HasPopup => expandedParentItem is { HasChildren: true };
	public string PopupTitle => expandedParentItem?.Title ?? string.Empty;
	public IEnumerable<PosNavItem> PopupItems =>
		expandedParentItem is null ? Array.Empty<PosNavItem>() : expandedParentItem.Children;
	public Thickness PopupMargin => new(0, CalculatePopupTop(), 0, 0);

	// External popup positioning: used when popup is rendered outside the sidebar
	// at the layout root level to avoid clipping by adjacent columns.
	public Thickness PopupOuterMargin
	{
		get
		{
			try
			{
				// Approximate layout: padding-top(16) + brand(52) + brand-margin(20) + index*60
				var top = 16 + 52 + 20 + CalculatePopupTop();
				var left = (IsCollapsed ? 76 : 240) + 4;
				return new Thickness(left, top, 0, 0);
			}
			catch
			{
				// Fallback during initialization
				return new Thickness(80, 88, 0, 0);
			}
		}
	}
	public double PopupTranslationX
	{
		get
		{
			if (PopupWithinBounds)
			{
				return IsCollapsed ? 52 : 40;
			}

			if (Width <= 0)
			{
				return IsCollapsed ? 56 : 228;
			}

			return Math.Max(52, Width - 24);
		}
	}
	public double PopupWidth
	{
		get
		{
			if (PopupWithinBounds && Width > 0)
			{
				return Math.Max(150, Width - 64);
			}

			return 180;
		}
	}

	private static void OnCollapsedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is PosSidebarView sidebar)
		{
			sidebar.UpdateCollapseState();
		}
	}

	private static void OnNavItemsChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is PosSidebarView sidebar)
		{
			sidebar.DetachNavItems(oldValue as IEnumerable);
			sidebar.AttachNavItems(newValue as IEnumerable);
			sidebar.UpdatePopupState();
		}
	}

	private void UpdateCollapseState()
	{
		OnPropertyChanged(nameof(ShowLabels));
		OnPropertyChanged(nameof(ToggleIcon));
		OnPropertyChanged(nameof(HeaderHorizontalOptions));
		OnPropertyChanged(nameof(SidebarPadding));
		OnPropertyChanged(nameof(NavListPadding));
		OnPropertyChanged(nameof(PopupTranslationX));
		OnPropertyChanged(nameof(PopupWidth));
		OnPropertyChanged(nameof(PopupOuterMargin));
		UpdatePopupState();
	}

	private void OnSidebarSizeChanged(object? sender, EventArgs e)
	{
		OnPropertyChanged(nameof(PopupTranslationX));
		OnPropertyChanged(nameof(PopupWidth));
		OnPropertyChanged(nameof(PopupMargin));
	}

	private void AttachNavItems(IEnumerable? items)
	{
		if (items is ObservableCollection<PosNavItem> collection)
		{
			observedNavItems = collection;
			observedNavItems.CollectionChanged += OnNavCollectionChanged;
		}

		foreach (var item in items?.OfType<PosNavItem>() ?? [])
		{
			SubscribeItem(item);
		}
	}

	private void DetachNavItems(IEnumerable? items)
	{
		if (observedNavItems != null)
		{
			observedNavItems.CollectionChanged -= OnNavCollectionChanged;
			observedNavItems = null;
		}

		foreach (var item in items?.OfType<PosNavItem>() ?? [])
		{
			UnsubscribeItem(item);
		}
	}

	private void OnNavCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		foreach (var item in e.OldItems?.OfType<PosNavItem>() ?? [])
		{
			UnsubscribeItem(item);
		}

		foreach (var item in e.NewItems?.OfType<PosNavItem>() ?? [])
		{
			SubscribeItem(item);
		}

		UpdatePopupState();
	}

	private void SubscribeItem(PosNavItem item)
	{
		if (!subscribedItems.Add(item))
		{
			return;
		}

		item.PropertyChanged += OnNavItemPropertyChanged;
		item.Children.CollectionChanged += OnChildCollectionChanged;

		foreach (var child in item.Children)
		{
			SubscribeItem(child);
		}
	}

	private void UnsubscribeItem(PosNavItem item)
	{
		if (!subscribedItems.Remove(item))
		{
			return;
		}

		item.PropertyChanged -= OnNavItemPropertyChanged;
		item.Children.CollectionChanged -= OnChildCollectionChanged;

		foreach (var child in item.Children)
		{
			UnsubscribeItem(child);
		}
	}

	private void OnChildCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		foreach (var item in e.OldItems?.OfType<PosNavItem>() ?? [])
		{
			UnsubscribeItem(item);
		}

		foreach (var item in e.NewItems?.OfType<PosNavItem>() ?? [])
		{
			SubscribeItem(item);
		}

		UpdatePopupState();
	}

	private void OnNavItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName is nameof(PosNavItem.IsExpanded) or nameof(PosNavItem.IsActive))
		{
			UpdatePopupState();
		}
	}

	private void UpdatePopupState()
	{
		var newExpandedItem = NavItems?
			.OfType<PosNavItem>()
			.FirstOrDefault(item => item.HasChildren && item.IsExpanded);

		if (ReferenceEquals(expandedParentItem, newExpandedItem))
		{
			OnPropertyChanged(nameof(PopupItems));
			return;
		}

		expandedParentItem = newExpandedItem;
		OnPropertyChanged(nameof(HasPopup));
		OnPropertyChanged(nameof(PopupTitle));
		OnPropertyChanged(nameof(PopupItems));
		OnPropertyChanged(nameof(PopupMargin));
		OnPropertyChanged(nameof(PopupOuterMargin));
	}

	private double CalculatePopupTop()
	{
		if (expandedParentItem is null)
		{
			return 0;
		}

		try
		{
			var items = NavItems?.OfType<PosNavItem>().ToList() ?? [];
			var index = items.IndexOf(expandedParentItem);
			return index < 0 ? 0 : index * 60;
		}
		catch
		{
			return 0;
		}
	}
}
