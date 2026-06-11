using System.Collections.ObjectModel;
using System.Windows.Input;
using EnterprisePOS.Core.Data.Models;
using EnterprisePOS.Core.Data.Repositories;
using EnterprisePOS.Helpers;

namespace EnterprisePOS.Features.Categories.ViewModels;

public class CategoryListViewModel : BaseViewModel
{
    private readonly ProductCategoryRepository _categoryRepository;
    private bool _isGridView = true;

    public ObservableCollection<ProductCategory> Categories { get; } = new();

    public bool IsGridView
    {
        get => _isGridView;
        set => SetProperty(ref _isGridView, value);
    }

    public ICommand AddCategoryCommand { get; }
    public ICommand EditCategoryCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ToggleViewCommand { get; }

    public CategoryListViewModel(ProductCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        AddCategoryCommand = new Command(async () => await Shell.Current.GoToAsync("CategoryEditor"));
        EditCategoryCommand = new Command<ProductCategory>(async (cat) => await Shell.Current.GoToAsync($"CategoryEditor?id={cat.Id}"));
        RefreshCommand = new Command(async () => await LoadCategoriesAsync());
        ToggleViewCommand = new Command(() => IsGridView = !IsGridView);

        _ = LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

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
        finally
        {
            IsBusy = false;
        }
    }
}
