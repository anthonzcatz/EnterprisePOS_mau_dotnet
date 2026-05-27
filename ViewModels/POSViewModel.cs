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
	private double availableWidth = 0;

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
		NavigateCommand = new Command<PosNavItem>(Navigate);
	}

	public ObservableCollection<Product> Products { get; } = [];
	public ObservableCollection<CartItem> CartItems { get; } = [];
	public ObservableCollection<ProductCategory> Categories { get; } = [];
	public ObservableCollection<PosNavItem> NavItems { get; } = [];

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
	public Command<PosNavItem> NavigateCommand { get; }

	public string CashierName { get; } = "Antonella";
	public string CashierRole { get; } = "Cashier";
	public string CashierImageUrl { get; } = "https://picsum.photos/seed/cashier/80/80";

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
		NavItems.Add(new PosNavItem { Key = "home",      Title = "Home",      Glyph = "⌂" });
		NavItems.Add(new PosNavItem { Key = "pos",       Title = "POS",       Glyph = "▦",  IsActive = true });
		NavItems.Add(new PosNavItem { Key = "orders",    Title = "Orders",    Glyph = "🛒" });
		NavItems.Add(new PosNavItem { Key = "history",   Title = "History",   Glyph = "🕐" });
		NavItems.Add(new PosNavItem { Key = "customers", Title = "Customers", Glyph = "👤" });
		NavItems.Add(new PosNavItem { Key = "links",     Title = "Links",     Glyph = "🔗" });
	}

	private void LoadCategories()
	{
		Categories.Clear();
		Categories.Add(new ProductCategory { Key = "all", Name = "All Decorations", Icon = "▦", IsSelected = true });
		Categories.Add(new ProductCategory { Key = "plants", Name = "Artificial Plants", Icon = "🌿" });
		Categories.Add(new ProductCategory { Key = "flowers", Name = "Artificial Flowers", Icon = "🌸" });
		Categories.Add(new ProductCategory { Key = "pots", Name = "Plant Pots", Icon = "🪴" });
		Categories.Add(new ProductCategory { Key = "vases", Name = "Vases & Bowls", Icon = "🏺" });
		Categories.Add(new ProductCategory { Key = "candles", Name = "Candles", Icon = "🕯" });
		Categories.Add(new ProductCategory { Key = "lights", Name = "String Lights", Icon = "💡" });
		Categories.Add(new ProductCategory { Key = "tools", Name = "Garden Tools", Icon = "🔧" });
		Categories.Add(new ProductCategory { Key = "seasonal", Name = "Seasonal", Icon = "🎄" });
		SelectedCategoryKey = "all";
	}

	private void ToggleSidebar()
	{
		IsSidebarCollapsed = !IsSidebarCollapsed;
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

	private static void ChangePaymentMethod()
	{
		// TODO: Show payment method selection
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
			"Payment integration can connect here (card, cash, e-wallet).",
			"OK");
	}
}
