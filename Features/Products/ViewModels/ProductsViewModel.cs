using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using EnterprisePOS.Core.Data.Models;
using EnterprisePOS.Core.Data.Repositories;
using EnterprisePOS.Helpers;
using EnterprisePOS.Navigation;

namespace EnterprisePOS.Features.Products.ViewModels;

public class ProductsViewModel : BaseViewModel
{
    private readonly ProductRepository _productRepository;
    private readonly ProductCategoryRepository _categoryRepository;
    private string _searchQuery = string.Empty;
    private bool _isGridView = true;
    private ProductCategory? _selectedCategory;

    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<ProductCategory> Categories { get; } = new();

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
            {
                _ = LoadProductsAsync();
            }
        }
    }

    public ProductCategory? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                _ = LoadProductsAsync();
            }
        }
    }

    public bool IsGridView
    {
        get => _isGridView;
        set => SetProperty(ref _isGridView, value);
    }

    public ICommand SearchCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand AddProductCommand { get; }
    public ICommand EditProductCommand { get; }
    public ICommand ToggleViewCommand { get; }
    public ICommand ClearFiltersCommand { get; }

    public ProductsViewModel(ProductRepository productRepository, ProductCategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;

        SearchCommand = new Command(async () => await LoadProductsAsync());
        RefreshCommand = new Command(async () => await LoadInitialDataAsync());
        ToggleViewCommand = new Command(() => IsGridView = !IsGridView);
        AddProductCommand = new Command(async () => 
        {
            try
            {
                Console.WriteLine($"[ProductsViewModel] Navigating to {Routes.ProductEditor}");
                await Shell.Current.GoToAsync(Routes.ProductEditor);
                Console.WriteLine($"[ProductsViewModel] Navigation completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProductsViewModel] Navigation error: {ex.Message}");
            }
        });
        EditProductCommand = new Command<Product>(async (product) => await EditProductAsync(product));
        ClearFiltersCommand = new Command(ClearFilters);

        _ = LoadInitialDataAsync();
    }

    private void ClearFilters()
    {
        SearchQuery = string.Empty;
        SelectedCategory = null;
    }

    private async Task LoadInitialDataAsync()
    {
        await LoadCategoriesAsync();
        await LoadProductsAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            Categories.Clear();
            foreach (var cat in categories)
            {
                Categories.Add(cat);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading categories: {ex.Message}");
        }
    }

    private async Task EditProductAsync(Product product)
    {
        if (product == null) 
        {
            Console.WriteLine($"[ProductsViewModel] EditProductAsync called with null product");
            return;
        }
        Console.WriteLine($"[ProductsViewModel] Editing product: {product.Name} (ID: {product.Id})");
        await Shell.Current.GoToAsync($"{Routes.ProductEditor}?id={product.Id}");
        Console.WriteLine($"[ProductsViewModel] Navigation to ProductEditor completed");
    }

    private async Task LoadProductsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            Console.WriteLine($"[ProductsViewModel] Loading products...");
            var products = await _productRepository.GetAllAsync();
            Console.WriteLine($"[ProductsViewModel] Retrieved {products.Count()} products from database");
            
            Products.Clear();
            foreach (var product in products)
            {
                bool matchesSearch = string.IsNullOrWhiteSpace(SearchQuery) || 
                                   product.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                   product.Code.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);

                bool matchesCategory = SelectedCategory == null || product.CategoryId == SelectedCategory.Id;

                if (matchesSearch && matchesCategory)
                {
                    Products.Add(product);
                    Console.WriteLine($"[ProductsViewModel] Added product: {product.Name} (ID: {product.Id})");
                }
            }
            Console.WriteLine($"[ProductsViewModel] Total products displayed: {Products.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading products: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
