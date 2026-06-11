using System.Windows.Input;
using System.Collections.ObjectModel;
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
    private readonly UnitRepository _unitRepository;
    private int? _productId;
    private string _name = string.Empty;
    private string _code = string.Empty;
    private string? _description;
    private decimal _costPrice;
    private decimal _sellingPrice;
    private int _categoryId;
    private int? _unitId;
    private bool _isActive = true;
    private bool _isTaxable = true;
    private string? _imageUrl;
    private bool _useImage = true;
    private string _posColor = "#356AE6";
    private string _posShape = "Square";
    private ProductCategory? _selectedCategory;
    private Unit? _selectedUnit;

    // Validation error properties
    private string _nameError = string.Empty;
    private string _codeError = string.Empty;
    private string _costPriceError = string.Empty;
    private string _sellingPriceError = string.Empty;
    private string _categoryError = string.Empty;
    private string _unitError = string.Empty;
    private string _formError = string.Empty;

    public ObservableCollection<ProductVariantViewModel> Variants { get; } = new();
    public List<ProductCategory> Categories { get; private set; } = new();
    public List<Unit> Units { get; private set; } = new();

    public List<string> AvailableShapes { get; } = new() { "Square", "Circle", "Rounded", "Hexagon", "Pill" };
    public List<string> AvailableColors { get; } = new() 
    { 
        "#356AE6", "#E63535", "#35E635", "#E6E635", 
        "#E635E6", "#35E6E6", "#1F2A3D", "#73819A" 
    };

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand AddVariantCommand { get; }
    public ICommand RemoveVariantCommand { get; }
    public ICommand UploadImageCommand { get; }
    public ICommand RemoveImageCommand { get; }
    public ICommand SetColorCommand { get; }
    public ICommand SetShapeCommand { get; }

    public ProductEditorViewModel(
        ProductRepository productRepository, 
        ProductCategoryRepository categoryRepository,
        UnitRepository unitRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitRepository = unitRepository;

        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        AddVariantCommand = new Command(AddVariant);
        RemoveVariantCommand = new Command<ProductVariantViewModel>(RemoveVariant);
        UploadImageCommand = new Command(async () => await UploadImageAsync());
        RemoveImageCommand = new Command(() => ImageUrl = null);
        SetColorCommand = new Command<string>(color => PosColor = color);
        SetShapeCommand = new Command<string>(shape => PosShape = shape);

        _ = LoadInitialDataAsync();
    }

    private async Task LoadInitialDataAsync()
    {
        await Task.WhenAll(LoadCategoriesAsync(), LoadUnitsAsync());
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

    private async Task LoadUnitsAsync()
    {
        try
        {
            var units = await _unitRepository.GetAllAsync();
            Units = units.ToList();
            OnPropertyChanged(nameof(Units));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading units: {ex.Message}");
        }
    }

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

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string Code { get => _code; set => SetProperty(ref _code, value); }
    public string? Description { get => _description; set => SetProperty(ref _description, value); }
    public decimal CostPrice { get => _costPrice; set => SetProperty(ref _costPrice, value); }
    public decimal SellingPrice { get => _sellingPrice; set => SetProperty(ref _sellingPrice, value); }
    public int CategoryId { get => _categoryId; set => SetProperty(ref _categoryId, value); }
    public int? UnitId { get => _unitId; set => SetProperty(ref _unitId, value); }
    public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
    public bool IsTaxable { get => _isTaxable; set => SetProperty(ref _isTaxable, value); }
    public string? ImageUrl { get => _imageUrl; set => SetProperty(ref _imageUrl, value); }
    public bool UseImage { get => _useImage; set => SetProperty(ref _useImage, value); }
    public string PosColor { get => _posColor; set => SetProperty(ref _posColor, value); }
    public string PosShape { get => _posShape; set => SetProperty(ref _posShape, value); }

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

    public Unit? SelectedUnit
    {
        get => _selectedUnit;
        set
        {
            if (SetProperty(ref _selectedUnit, value))
            {
                UnitId = value?.Id;
            }
        }
    }

    // Validation error properties
    public string NameError { get => _nameError; set => SetError(ref _nameError, value, nameof(NameError)); }
    public string CodeError { get => _codeError; set => SetError(ref _codeError, value, nameof(CodeError)); }
    public string CostPriceError { get => _costPriceError; set => SetError(ref _costPriceError, value, nameof(CostPriceError)); }
    public string SellingPriceError { get => _sellingPriceError; set => SetError(ref _sellingPriceError, value, nameof(SellingPriceError)); }
    public string CategoryError { get => _categoryError; set => SetError(ref _categoryError, value, nameof(CategoryError)); }
    public string UnitError { get => _unitError; set => SetError(ref _unitError, value, nameof(UnitError)); }
    public string FormError { get => _formError; set => SetError(ref _formError, value, nameof(FormError)); }

    private void SetError(ref string field, string value, string propertyName)
    {
        field = value;
        OnPropertyChanged(propertyName);
    }

    private void AddVariant()
    {
        Variants.Add(new ProductVariantViewModel());
    }

    private void RemoveVariant(ProductVariantViewModel variant)
    {
        if (variant != null)
        {
            Variants.Remove(variant);
        }
    }

    private async Task UploadImageAsync()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select a product image",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                // Copy image to cache directory
                var tempPath = Path.Combine(FileSystem.CacheDirectory, $"product_{Guid.NewGuid()}{Path.GetExtension(result.FullPath)}");
                File.Copy(result.FullPath, tempPath, true);
                ImageUrl = tempPath;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Image pick error: {ex.Message}");
        }
    }

    private async Task LoadProductAsync(int id)
    {
        Console.WriteLine($"[ProductEditor] LoadProductAsync called with ID: {id}");
        IsBusy = true;
        try
        {
            var product = await _productRepository.GetByIdAsync(id);
            Console.WriteLine($"[ProductEditor] Retrieved product: {product?.Name ?? "null"}");
            if (product != null)
            {
                Name = product.Name;
                Code = product.Code;
                Description = product.Description;
                CostPrice = product.CostPrice;
                SellingPrice = product.SellingPrice;
                CategoryId = product.CategoryId;
                UnitId = product.UnitId;
                IsActive = product.IsActive;
                IsTaxable = product.IsTaxable;
                ImageUrl = product.ImageUrl;
                UseImage = product.UseImage;
                PosColor = product.PosColor ?? "#356AE6";
                PosShape = product.PosShape ?? "Square";
                
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == CategoryId);
                SelectedUnit = Units.FirstOrDefault(u => u.Id == UnitId);

                Console.WriteLine($"[ProductEditor] Loaded product data - Name: {Name}, Code: {Code}, CategoryId: {CategoryId}, UnitId: {UnitId}");

                Variants.Clear();
                foreach (var v in product.ProductVariants)
                {
                    Variants.Add(new ProductVariantViewModel
                    {
                        Id = v.Id,
                        Name = v.Name,
                        PriceAdjustment = v.PriceAdjustment,
                        IsActive = v.IsActive
                    });
                }
                Console.WriteLine($"[ProductEditor] Loaded {Variants.Count} variants");
            }
            else
            {
                Console.WriteLine($"[ProductEditor] Product not found with ID: {id}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ProductEditor] Error loading product: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        if (IsBusy) return;

        Console.WriteLine($"[ProductEditor] SaveAsync called - ProductId: {ProductId}");
        
        var validationErrors = ValidateFields();
        Console.WriteLine($"[ProductEditor] Validation errors: {validationErrors.Count}");
        if (validationErrors.Count > 0)
        {
            FormError = "Please correct the errors before saving.";
            Console.WriteLine($"[ProductEditor] FormError set: {FormError}");
            return;
        }

        IsBusy = true;
        try
        {
            if (!ProductId.HasValue && await _productRepository.CodeExistsAsync(Code))
            {
                CodeError = "Product code already exists. Please use a unique SKU.";
                FormError = "This product code is already in use.";
                Console.WriteLine($"[ProductEditor] Code already exists: {Code}");
                return;
            }

            Product product;
            if (ProductId.HasValue)
            {
                product = await _productRepository.GetByIdAsync(ProductId.Value) ?? new Product();
                Console.WriteLine($"[ProductEditor] Updating existing product: {ProductId}");
            }
            else
            {
                product = new Product();
                Console.WriteLine($"[ProductEditor] Creating new product");
            }

            product.Name = Name.Trim();
            product.Code = Code.Trim().ToUpper();
            product.Description = Description?.Trim();
            product.CostPrice = CostPrice;
            product.SellingPrice = SellingPrice;
            product.CategoryId = CategoryId;
            product.UnitId = UnitId;
            product.IsActive = IsActive;
            product.IsTaxable = IsTaxable;
            product.ImageUrl = ImageUrl;
            product.UseImage = UseImage;
            product.PosColor = PosColor;
            product.PosShape = PosShape;

            Console.WriteLine($"[ProductEditor] Product data - Name: {product.Name}, Code: {product.Code}, CategoryId: {CategoryId}, UnitId: {UnitId}");

            // Handle variants
            product.ProductVariants.Clear();
            foreach (var vvm in Variants)
            {
                product.ProductVariants.Add(new ProductVariant
                {
                    Id = vvm.Id ?? 0,
                    ProductId = product.Id,
                    Name = vvm.Name,
                    PriceAdjustment = vvm.PriceAdjustment,
                    IsActive = vvm.IsActive
                });
            }

            if (ProductId.HasValue)
            {
                await _productRepository.UpdateAsync(product);
                Console.WriteLine($"[ProductEditor] Product updated");
            }
            else
            {
                await _productRepository.AddAsync(product);
                Console.WriteLine($"[ProductEditor] Product added with ID: {product.Id}");
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            FormError = $"Error saving product: {ex.Message}";
            Console.WriteLine($"[ProductEditor] Error saving product: {ex.Message}");
            Console.WriteLine($"[ProductEditor] Stack trace: {ex.StackTrace}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private List<string> ValidateFields()
    {
        var errors = new List<string>();
        NameError = CodeError = CostPriceError = SellingPriceError = CategoryError = UnitError = FormError = string.Empty;

        if (string.IsNullOrWhiteSpace(Name)) { errors.Add("Name required"); NameError = "Product Name is required."; }
        if (string.IsNullOrWhiteSpace(Code)) { errors.Add("Code required"); CodeError = "Product Code is required."; }
        if (CostPrice < 0) { errors.Add("Cost negative"); CostPriceError = "Cost Price cannot be negative."; }
        if (SellingPrice < 0) { errors.Add("Price negative"); SellingPriceError = "Selling Price cannot be negative."; }
        if (CategoryId <= 0) { errors.Add("Category required"); CategoryError = "Please select a category."; }
        if (UnitId == null || UnitId <= 0) { errors.Add("Unit required"); UnitError = "Please select a base unit."; }

        return errors;
    }
}

public class ProductVariantViewModel : BaseViewModel
{
    private int? _id;
    private string _name = string.Empty;
    private decimal _priceAdjustment;
    private bool _isActive = true;

    public int? Id { get => _id; set => SetProperty(ref _id, value); }
    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public decimal PriceAdjustment { get => _priceAdjustment; set => SetProperty(ref _priceAdjustment, value); }
    public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
}
