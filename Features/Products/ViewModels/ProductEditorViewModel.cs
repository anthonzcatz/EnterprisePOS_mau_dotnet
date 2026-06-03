using System.Windows.Input;
using Microsoft.Maui.Controls;
using EnterprisePOS.Core.Data.Models;
using EnterprisePOS.Core.Data.Repositories;
using EnterprisePOS.Helpers;

namespace EnterprisePOS.Features.Products.ViewModels;

[QueryProperty(nameof(ProductId), "id")]
public class ProductEditorViewModel : BaseViewModel
{
    private readonly ProductRepository _productRepository;
    private readonly ProductCategoryRepository _categoryRepository;
    private int? _productId;
    private string _name = string.Empty;
    private string _code = string.Empty;
    private string? _description;
    private decimal _costPrice;
    private decimal _sellingPrice;
    private int _categoryId;
    private bool _isActive = true;
    private bool _isTaxable = true;
    private ProductCategory? _selectedCategory;

    public int? ProductId
    {
        get => _productId;
        set
        {
            _productId = value;
            if (_productId.HasValue)
            {
                _ = LoadProductAsync(_productId.Value);
            }
        }
    }

    public string EditorTitle => ProductId.HasValue ? "Edit Product" : "New Product";

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Code
    {
        get => _code;
        set => SetProperty(ref _code, value);
    }

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public decimal CostPrice
    {
        get => _costPrice;
        set => SetProperty(ref _costPrice, value);
    }

    public decimal SellingPrice
    {
        get => _sellingPrice;
        set => SetProperty(ref _sellingPrice, value);
    }

    public int CategoryId
    {
        get => _categoryId;
        set => SetProperty(ref _categoryId, value);
    }

    public ProductCategory? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                CategoryId = value?.Id ?? 0;
            }
        }
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public bool IsTaxable
    {
        get => _isTaxable;
        set => SetProperty(ref _isTaxable, value);
    }

    public List<ProductCategory> Categories { get; private set; } = new();

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public ProductEditorViewModel(ProductRepository productRepository, ProductCategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;

        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

        _ = LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            Categories = categories.ToList();
            OnPropertyChanged(nameof(Categories));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading categories: {ex.Message}");
        }
    }

    private async Task LoadProductAsync(int id)
    {
        IsBusy = true;
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                Name = product.Name;
                Code = product.Code;
                Description = product.Description;
                CostPrice = product.CostPrice;
                SellingPrice = product.SellingPrice;
                CategoryId = product.CategoryId;
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == CategoryId);
                IsActive = product.IsActive;
                IsTaxable = product.IsTaxable;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading product: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Code))
        {
            // Simple validation
            return;
        }

        IsBusy = true;
        try
        {
            Product product;
            if (ProductId.HasValue)
            {
                product = await _productRepository.GetByIdAsync(ProductId.Value) ?? new Product();
            }
            else
            {
                product = new Product();
            }

            product.Name = Name;
            product.Code = Code;
            product.Description = Description;
            product.CostPrice = CostPrice;
            product.SellingPrice = SellingPrice;
            product.CategoryId = CategoryId;
            product.IsActive = IsActive;
            product.IsTaxable = IsTaxable;

            if (ProductId.HasValue)
            {
                await _productRepository.UpdateAsync(product);
            }
            else
            {
                await _productRepository.AddAsync(product);
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving product: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
