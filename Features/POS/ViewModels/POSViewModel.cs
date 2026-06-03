using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using EnterprisePOS.Helpers;
using EnterprisePOS.Interfaces;
using EnterprisePOS.Features.POS.Models;

namespace EnterprisePOS.Features.POS.ViewModels;

public sealed class POSViewModel : BaseViewModel
{
	private const double SidebarExpandedWidth = 240;
	private const double SidebarCollapsedWidth = 76;

	private readonly IPosService posService;
	private string searchText = string.Empty;
	private string selectedCategoryKey = "all";
	private bool isSidebarCollapsed = true;
	private bool isMobileSidebarOpen;
	private bool isProfileMenuOpen;
	private bool isMobileCartOpen;
	private bool isMobileCheckoutOpen;
	private double availableWidth = 0;
	private int paymentMethodIndex = 0;
	private int cartItemCount = 0;

	private static readonly (string Label, string Icon)[] PaymentMethods =
	[
		("Cash", "💵"),
		("GCash", "📱"),
		("Credit Card", "💳")
	];

	public POSViewModel(IPosService posService)
	{
		this.posService = posService;
		AddToCartCommand = new Command<Product>(AddToCart);
		IncreaseQuantityCommand = new Command<CartItem>(IncreaseQuantity);
		DecreaseQuantityCommand = new Command<CartItem>(DecreaseQuantity);
		DeleteCartItemCommand = new Command<CartItem>(DeleteCartItem);
		SelectCategoryCommand = new Command<ProductCategory>(SelectCategory);
		PlaceOrderCommand = new Command(async () => await PlaceOrderAsync());
		ResetOrderCommand = new Command(ResetOrder);
		ContinueCommand = new Command(async () => await ContinueAsync());
		ChangePaymentMethodCommand = new Command(ChangePaymentMethod);
		ToggleSidebarCommand = new Command(ToggleSidebar);
		OpenMobileSidebarCommand = new Command(OpenMobileSidebar);
		CloseMobileSidebarCommand = new Command(CloseMobileSidebar);
		ToggleProfileMenuCommand = new Command(ToggleProfileMenu);
		CloseProfileMenuCommand = new Command(CloseProfileMenu);
		ProfileMenuCommand = new Command<PosNavItem>(SelectProfileMenuItem);
		NavigateCommand = new Command<PosNavItem>(Navigate);
		ToggleMobileCartCommand = new Command(ToggleMobileCart);
		ToggleMobileCheckoutCommand = new Command(ToggleMobileCheckout);

		Products.CollectionChanged += OnProductsCollectionChanged;
		CartItems.CollectionChanged += OnCartItemsCollectionChanged;

		NotifyCartStateChanged();
	}

	public ObservableCollection<Product> Products { get; } = [];
	public ObservableCollection<CartItem> CartItems { get; } = [];
	public ObservableCollection<ProductCategory> Categories { get; } = [];
	public ObservableCollection<PosNavItem> NavItems { get; } = [];
	public ObservableCollection<PosNavItem> ProfileMenuItems { get; } = [];

	public Command<Product> AddToCartCommand { get; }
	public Command<CartItem> IncreaseQuantityCommand { get; }
	public Command<CartItem> DecreaseQuantityCommand { get; }
	public Command<CartItem> DeleteCartItemCommand { get; }
	public Command<ProductCategory> SelectCategoryCommand { get; }
	public Command PlaceOrderCommand { get; }
	public Command ResetOrderCommand { get; }
	public Command ContinueCommand { get; }
	public Command ChangePaymentMethodCommand { get; }
	public Command ToggleSidebarCommand { get; }
	public Command OpenMobileSidebarCommand { get; }
	public Command CloseMobileSidebarCommand { get; }
	public Command ToggleProfileMenuCommand { get; }
	public Command CloseProfileMenuCommand { get; }
	public Command<PosNavItem> ProfileMenuCommand { get; }
	public Command<PosNavItem> NavigateCommand { get; }
	public Command ToggleMobileCartCommand { get; }
	public Command ToggleMobileCheckoutCommand { get; }

	public double MobileSidebarWidth => Math.Min(260, availableWidth > 0 ? availableWidth * 0.7 : 240);

	public string StoreName { get; } = "Dos Avenue";
	public string StoreTagline { get; } = "Coffee · Pastries · Good days";

	public string CashierName { get; } = "Antonella";
	public string CashierRole { get; } = "Cashier";
	public string CashierInitials =>
		string.Concat(CashierName
			.Split(' ', StringSplitOptions.RemoveEmptyEntries)
			.Select(part => char.ToUpperInvariant(part[0])))
		[..Math.Min(2, CashierName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length)];

	public string PaymentMethodLabel => PaymentMethods[paymentMethodIndex].Label;
	public string PaymentMethodIcon => PaymentMethods[paymentMethodIndex].Icon;

	public bool IsSidebarCollapsed
	{
		get => isSidebarCollapsed;
		set
		{
			if (SetProperty(ref isSidebarCollapsed, value))
			{
				OnPropertyChanged(nameof(SidebarGridLength));
			}
		}
	}

	public GridLength SidebarGridLength =>
		new(IsSidebarCollapsed ? SidebarCollapsedWidth : SidebarExpandedWidth);

	public bool IsMobileSidebarOpen
	{
		get => isMobileSidebarOpen;
		set => SetProperty(ref isMobileSidebarOpen, value);
	}

	public bool IsProfileMenuOpen
	{
		get => isProfileMenuOpen;
		set => SetProperty(ref isProfileMenuOpen, value);
	}

	public bool IsMobileCartOpen
	{
		get => isMobileCartOpen;
		set
		{
			SetProperty(ref isMobileCartOpen, value);
			OnPropertyChanged(nameof(IsContentVisible));
		}
	}

	public bool IsContentVisible => !IsMobileCartOpen;

	public bool IsMobileCheckoutOpen
	{
		get => isMobileCheckoutOpen;
		set => SetProperty(ref isMobileCheckoutOpen, value);
	}

	public int CartItemCount
	{
		get => cartItemCount;
		private set => SetProperty(ref cartItemCount, value);
	}

	public string SelectedCategoryName => Categories.FirstOrDefault(category => category.IsSelected)?.Name ?? "All Menu";

	public int ProductGridSpan
	{
		get
		{
			var catalogWidth = GetCatalogWidthEstimate();
			if (catalogWidth < 360) return 1;
			if (catalogWidth < 700) return 2;
			if (catalogWidth < 980) return 3;
			return 4;
		}
	}

	public double CartColumnWidth
	{
		get
		{
			if (availableWidth < 1100) return 320;
			if (availableWidth < 1400) return 344;
			return 380;
		}
	}

	public double DesktopIdentityWidth
	{
		get
		{
			if (availableWidth < 1180) return 150;
			if (availableWidth < 1380) return 180;
			return 220;
		}
	}

	public bool ShowDesktopUtilityActions => availableWidth >= 1180;

	public int MobileProductGridSpan => availableWidth > 0 && availableWidth < 430 ? 1 : 2;

	public double MobileProductListHeight
	{
		get
		{
			if (Products.Count == 0)
			{
				return 0;
			}

			var rows = (int)Math.Ceiling(Products.Count / (double)MobileProductGridSpan);
			var rowHeight = MobileProductGridSpan == 1 ? 246 : 228;
			return rows * rowHeight + Math.Max(0, rows - 1) * 10;
		}
	}

	public double MobileCartListHeight
	{
		get
		{
			if (CartItems.Count == 0)
			{
				return 0;
			}

			return CartItems.Count * 92 + Math.Max(0, CartItems.Count - 1) * 8;
		}
	}

	public double CheckoutModalWidth
	{
		get
		{
			if (availableWidth <= 0)
			{
				return 380;
			}

			return Math.Clamp(availableWidth - 24, 280, 420);
		}
	}

	public void UpdateAvailableWidth(double width)
	{
		availableWidth = width;
		OnPropertyChanged(nameof(ProductGridSpan));
		OnPropertyChanged(nameof(CartColumnWidth));
		OnPropertyChanged(nameof(DesktopIdentityWidth));
		OnPropertyChanged(nameof(ShowDesktopUtilityActions));
		OnPropertyChanged(nameof(MobileProductGridSpan));
		OnPropertyChanged(nameof(MobileProductListHeight));
		OnPropertyChanged(nameof(MobileCartListHeight));
		OnPropertyChanged(nameof(CheckoutModalWidth));
		OnPropertyChanged(nameof(MobileSidebarWidth));
	}

	public string SearchText
	{
		get => searchText;
		set
		{
			if (SetProperty(ref searchText, value))
			{
				ApplyFilter();
			}
		}
	}

	public string SelectedCategoryKey
	{
		get => selectedCategoryKey;
		private set => SetProperty(ref selectedCategoryKey, value);
	}

	public decimal SubTotal => CartItems.Sum(item => item.LineTotal);
	public decimal TaxAmount => Math.Round(SubTotal * 0.12m, 2);
	public decimal DiscountAmount => Math.Round(SubTotal * 0.10m, 2);
	public decimal TotalPayment => Math.Round(SubTotal + TaxAmount - DiscountAmount, 2);

	public async Task InitializeAsync()
	{
		if (Categories.Count > 0)
		{
			return;
		}

		IsBusy = true;
		try
		{
			LoadNavItems();
			LoadProfileMenuItems();
			LoadCategories();

			allProducts = (await posService.GetProductsAsync()).ToList();
			ApplyFilter();
		}
		finally
		{
			IsBusy = false;
		}
	}

	private List<Product> allProducts = [];

	private void LoadNavItems()
	{
		NavItems.Clear();
		NavItems.Add(new PosNavItem { Key = "home", Title = "Home", Glyph = "⌂" });
		NavItems.Add(new PosNavItem { Key = "pos", Title = "Register", Glyph = "⊞", IsActive = true });
		NavItems.Add(new PosNavItem { Key = "orders", Title = "Orders", Glyph = "🧾" });
		NavItems.Add(new PosNavItem { Key = "kitchen", Title = "Kitchen", Glyph = "🍳" });
		NavItems.Add(new PosNavItem { Key = "customers", Title = "Guests", Glyph = "👥" });
		NavItems.Add(new PosNavItem { Key = "reports", Title = "Reports", Glyph = "📊" });
	}

	private void LoadProfileMenuItems()
	{
		ProfileMenuItems.Clear();
		ProfileMenuItems.Add(new PosNavItem { Key = "shift-summary", Title = "Shift summary", Glyph = "⏱" });
		ProfileMenuItems.Add(new PosNavItem { Key = "sales-report", Title = "Sales report", Glyph = "📈" });
		ProfileMenuItems.Add(new PosNavItem { Key = "notes", Title = "Cashier notes", Glyph = "📝" });
		ProfileMenuItems.Add(new PosNavItem { Key = "logout", Title = "Sign out", Glyph = "↪" });
	}

	private void LoadCategories()
	{
		Categories.Clear();
		Categories.Add(new ProductCategory { Key = "all", Name = "All Menu", Icon = "☕", IsSelected = true });
		Categories.Add(new ProductCategory { Key = "coffee", Name = "Coffee", Icon = "☕" });
		Categories.Add(new ProductCategory { Key = "espresso", Name = "Espresso", Icon = "◉" });
		Categories.Add(new ProductCategory { Key = "tea", Name = "Tea", Icon = "🍵" });
		Categories.Add(new ProductCategory { Key = "pastries", Name = "Pastries", Icon = "🥐" });
		Categories.Add(new ProductCategory { Key = "sandwiches", Name = "Bites", Icon = "🥪" });
		Categories.Add(new ProductCategory { Key = "cold", Name = "Cold Drinks", Icon = "🧊" });
		Categories.Add(new ProductCategory { Key = "seasonal", Name = "Seasonal", Icon = "✨" });
		SelectedCategoryKey = "all";
		OnPropertyChanged(nameof(SelectedCategoryName));
	}

	private void ToggleSidebar()
	{
		IsSidebarCollapsed = !IsSidebarCollapsed;
	}

	private void OpenMobileSidebar()
	{
		IsSidebarCollapsed = false;
		IsProfileMenuOpen = false;
		IsMobileSidebarOpen = true;
	}

	private void CloseMobileSidebar()
	{
		IsMobileSidebarOpen = false;
	}

	private void ToggleProfileMenu()
	{
		IsMobileSidebarOpen = false;
		IsProfileMenuOpen = !IsProfileMenuOpen;
	}

	private void CloseProfileMenu()
	{
		IsProfileMenuOpen = false;
	}

	private void SelectProfileMenuItem(PosNavItem? item)
	{
		if (item is null)
		{
			return;
		}

		IsProfileMenuOpen = false;
	}

	private void Navigate(PosNavItem? item)
	{
		if (item is null)
		{
			return;
		}

		foreach (var nav in NavItems)
		{
			nav.IsActive = nav.Key == item.Key;
		}

		IsProfileMenuOpen = false;
		IsMobileSidebarOpen = false;
	}

	private void ToggleMobileCart()
	{
		IsMobileCartOpen = !IsMobileCartOpen;
	}

	private void ToggleMobileCheckout()
	{
		IsMobileCheckoutOpen = !IsMobileCheckoutOpen;
	}

	private void SelectCategory(ProductCategory? category)
	{
		if (category is null)
		{
			return;
		}

		SelectedCategoryKey = category.Key;
		foreach (var cat in Categories)
		{
			cat.IsSelected = cat.Key == category.Key;
		}

		OnPropertyChanged(nameof(SelectedCategoryName));
		ApplyFilter();
	}

	private void ApplyFilter()
	{
		Products.Clear();
		var query = allProducts.AsEnumerable();

		if (!string.Equals(SelectedCategoryKey, "all", StringComparison.OrdinalIgnoreCase))
		{
			query = query.Where(p => string.Equals(p.Category, SelectedCategoryKey, StringComparison.OrdinalIgnoreCase));
		}

		if (!string.IsNullOrWhiteSpace(SearchText))
		{
			query = query.Where(p =>
				p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
				p.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
				p.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
		}

		foreach (var product in query)
		{
			Products.Add(product);
		}
	}

	private void OnProductsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		OnPropertyChanged(nameof(MobileProductListHeight));
	}

	private void OnCartItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems is not null)
		{
			foreach (CartItem item in e.OldItems)
			{
				item.PropertyChanged -= OnCartItemPropertyChanged;
			}
		}

		if (e.NewItems is not null)
		{
			foreach (CartItem item in e.NewItems)
			{
				item.PropertyChanged += OnCartItemPropertyChanged;
			}
		}

		NotifyCartStateChanged();
	}

	private void OnCartItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName is nameof(CartItem.Quantity) or nameof(CartItem.LineTotal))
		{
			NotifyCartStateChanged();
		}
	}

	private void NotifyCartStateChanged()
	{
		CartItemCount = CartItems.Sum(item => item.Quantity);
		OnPropertyChanged(nameof(SubTotal));
		OnPropertyChanged(nameof(TaxAmount));
		OnPropertyChanged(nameof(DiscountAmount));
		OnPropertyChanged(nameof(TotalPayment));
		OnPropertyChanged(nameof(MobileCartListHeight));
	}

	private double GetCatalogWidthEstimate()
	{
		if (availableWidth <= 0)
		{
			return 720;
		}

		if (availableWidth >= LayoutBreakpoints.DesktopMin)
		{
			return Math.Max(
				320,
				availableWidth -
				(IsSidebarCollapsed ? SidebarCollapsedWidth : SidebarExpandedWidth) -
				CartColumnWidth -
				40);
		}

		return availableWidth - 24;
	}

	private void AddToCart(Product? product)
	{
		if (product is null)
		{
			return;
		}

		var existing = CartItems.FirstOrDefault(item => item.Name == product.Name);
		if (existing is null)
		{
			CartItems.Add(new CartItem
			{
				Name = product.Name,
				Description = product.Description,
				Image = product.Image,
				Price = product.Price,
				Quantity = 1
			});
		}
		else
		{
			existing.Quantity += 1;
		}
	}

	private void IncreaseQuantity(CartItem? item)
	{
		if (item is null)
		{
			return;
		}

		item.Quantity += 1;
	}

	private void DecreaseQuantity(CartItem? item)
	{
		if (item is null)
		{
			return;
		}

		if (item.Quantity <= 1)
		{
			CartItems.Remove(item);
		}
		else
		{
			item.Quantity -= 1;
		}
	}

	private void DeleteCartItem(CartItem? item)
	{
		if (item is null)
		{
			return;
		}

		CartItems.Remove(item);
	}

	private void ResetOrder()
	{
		CartItems.Clear();
	}

	private async Task ContinueAsync()
	{
		var page = Application.Current?.Windows.FirstOrDefault()?.Page;
		if (page is null)
		{
			return;
		}

		if (CartItems.Count == 0)
		{
			await page.DisplayAlertAsync(
				"Cart Empty",
				"Please add items to your cart before proceeding to checkout.",
				"OK");
			return;
		}

		await page.DisplayAlertAsync(
			"Continue",
			"Proceeding to payment...",
			"OK");
	}

	private void ChangePaymentMethod()
	{
		paymentMethodIndex = (paymentMethodIndex + 1) % PaymentMethods.Length;
		OnPropertyChanged(nameof(PaymentMethodLabel));
		OnPropertyChanged(nameof(PaymentMethodIcon));
	}

	private async Task PlaceOrderAsync()
	{
		var page = Application.Current?.Windows.FirstOrDefault()?.Page;
		if (page is null)
		{
			return;
		}

		await page.DisplayAlertAsync(
			"Order placed",
			"Payment integration can connect here (cash, card, e-wallet).",
			"OK");
	}
}
