using System.Windows.Input;
using EnterprisePOS.Core.Data.Models;
using EnterprisePOS.Core.Data.Repositories;
using EnterprisePOS.Helpers;

namespace EnterprisePOS.Features.Categories.ViewModels;

[QueryProperty(nameof(CategoryId), "id")]
public class CategoryEditorViewModel : BaseViewModel
{
    private readonly ProductCategoryRepository _categoryRepository;
    private int? _categoryId;
    private string _name = string.Empty;
    private string? _description;
    private int? _parentId;
    private bool _isActive = true;
    private int _sortOrder;

    public List<ProductCategory> ParentCategories { get; private set; } = new();
    private ProductCategory? _selectedParent;

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public CategoryEditorViewModel(ProductCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        
        _ = LoadParentCategoriesAsync();
    }

    public int? CategoryId
    {
        get => _categoryId;
        set
        {
            _categoryId = value;
            if (_categoryId.HasValue) _ = LoadCategoryAsync(_categoryId.Value);
        }
    }

    public string EditorTitle => CategoryId.HasValue ? "Edit Category" : "New Category";

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string? Description { get => _description; set => SetProperty(ref _description, value); }
    public int? ParentCategoryId { get => _parentId; set => SetProperty(ref _parentId, value); }
    public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
    public int SortOrder { get => _sortOrder; set => SetProperty(ref _sortOrder, value); }

    public ProductCategory? SelectedParent
    {
        get => _selectedParent;
        set
        {
            if (SetProperty(ref _selectedParent, value))
            {
                ParentCategoryId = value?.Id;
            }
        }
    }

    private async Task LoadParentCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        ParentCategories = categories.Where(c => c.Id != CategoryId).ToList();
        OnPropertyChanged(nameof(ParentCategories));
    }

    private async Task LoadCategoryAsync(int id)
    {
        var cat = await _categoryRepository.GetByIdAsync(id);
        if (cat != null)
        {
            Name = cat.Name;
            Description = cat.Description;
            ParentCategoryId = cat.ParentCategoryId;
            IsActive = cat.IsActive;
            SortOrder = cat.SortOrder;
            SelectedParent = ParentCategories.FirstOrDefault(c => c.Id == ParentCategoryId);
        }
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name)) return;

        IsBusy = true;
        try
        {
            var cat = CategoryId.HasValue ? await _categoryRepository.GetByIdAsync(CategoryId.Value) : new ProductCategory();
            if (cat == null) cat = new ProductCategory();

            cat.Name = Name.Trim();
            cat.Description = Description?.Trim();
            cat.ParentCategoryId = ParentCategoryId;
            cat.IsActive = IsActive;
            cat.SortOrder = SortOrder;

            if (CategoryId.HasValue) await _categoryRepository.UpdateAsync(cat);
            else await _categoryRepository.AddAsync(cat);

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
