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
    private string _searchQuery = string.Empty;
    private bool _isGridView = true;

    public ObservableCollection<Product> Products { get; } = new();

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

    public bool IsGridView
    {
        get => _isGridView;
        set => SetProperty(ref _isGridView, value);
    }

    public ICommand SearchCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand AddProductCommand { get; }
    public ICommand ToggleViewCommand { get; }

    public ProductsViewModel(ProductRepository productRepository)
    {
        _productRepository = productRepository;

        SearchCommand = new Command(async () => await LoadProductsAsync());
        RefreshCommand = new Command(async () => await LoadProductsAsync());
        ToggleViewCommand = new Command(() => IsGridView = !IsGridView);
        AddProductCommand = new Command(async () => await Shell.Current.GoToAsync(Routes.ProductEditor));

        _ = LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            var products = await _productRepository.GetAllAsync();
            
            Products.Clear();
            foreach (var product in products)
            {
                if (string.IsNullOrWhiteSpace(SearchQuery) || 
                    product.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    product.Code.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                {
                    Products.Add(product);
                }
            }
        }
        catch (Exception ex)
        {
            // Handle error
            Console.WriteLine($"Error loading products: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
