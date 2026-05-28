using System.Collections.ObjectModel;
using System.Windows.Input;
using EnterprisePOS.Helpers;
using EnterprisePOS.Interfaces;
using EnterprisePOS.Models;

namespace EnterprisePOS.ViewModels;

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
	private double availableWidth = 0;
	private int paymentMethodIndex = 0;

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

	public double MobileSidebarWidth => Math.Min(260, availableWidth > 0 ? availableWidth * 0.7 : 240);

	public string StoreName { get; } = "Dos Avenue";
	public string StoreTagline { get; } = "Coffee · Pastries · Good days";

	public string CashierName { get; } = "Antonella";
	public string CashierRole { get; } = "Cashier";
	public string CashierImageUrl { get; } = "https://picsum.photos/seed/cashier/80/80";

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

	public string SelectedCategoryName => Categories.FirstOrDefault(category => category.IsSelected)?.Name ?? "All Menu";

	public int ProductGridSpan
	{
		get
		{
			if (availableWidth < 600) return 1;
			if (availableWidth < 900) return 2;
			if (availableWidth < 1200) return 3;
			return 4;
		}
	}

	public void UpdateAvailableWidth(double width)
	{
		availableWidth = width;
		OnPropertyChanged(nameof(ProductGridSpan));
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
		NavItems.Add(new PosNavItem { Key = "home", Title = "Home", Glyph = "\uE80F" });
		NavItems.Add(new PosNavItem { Key = "pos", Title = "Register", Glyph = "\uE719", IsActive = true });
		NavItems.Add(new PosNavItem { Key = "orders", Title = "Orders", Glyph = "\uE7BF" });
		NavItems.Add(new PosNavItem { Key = "kitchen", Title = "Kitchen", Glyph = "\uE790" });
		NavItems.Add(new PosNavItem { Key = "customers", Title = "Guests", Glyph = "\uE77B" });
		NavItems.Add(new PosNavItem { Key = "reports", Title = "Reports", Glyph = "\uE9D2" });
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

		NotifyTotals();
	}

	private void IncreaseQuantity(CartItem? item)
	{
		if (item is null)
		{
			return;
		}

		item.Quantity += 1;
		NotifyTotals();
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

		NotifyTotals();
	}

	private void DeleteCartItem(CartItem? item)
	{
		if (item is null)
		{
			return;
		}

		CartItems.Remove(item);
		NotifyTotals();
	}

	private void ResetOrder()
	{
		CartItems.Clear();
		NotifyTotals();
	}

	private static async Task ContinueAsync()
	{
		var page = Application.Current?.Windows.FirstOrDefault()?.Page;
		if (page is null)
		{
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

	private void NotifyTotals()
	{
		OnPropertyChanged(nameof(SubTotal));
		OnPropertyChanged(nameof(TaxAmount));
		OnPropertyChanged(nameof(DiscountAmount));
		OnPropertyChanged(nameof(TotalPayment));
	}

	private static async Task PlaceOrderAsync()
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
