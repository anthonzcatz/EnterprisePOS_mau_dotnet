using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using EnterprisePOS.Core.Data.Models;
using EnterprisePOS.Core.Data.Repositories;
using EnterprisePOS.Helpers;

namespace EnterprisePOS.Features.Inventory.ViewModels;

public class InventoryViewModel : BaseViewModel
{
    private readonly ProductStockRepository _stockRepository;
    private string _searchQuery = string.Empty;
    private decimal _totalValuation;
    private int _lowStockCount;
    private int _outOfStockCount;

    public ObservableCollection<ProductStock> Stocks { get; } = new();

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
            {
                _ = LoadInventoryAsync();
            }
        }
    }

    public decimal TotalValuation
    {
        get => _totalValuation;
        set => SetProperty(ref _totalValuation, value);
    }

    public int LowStockCount
    {
        get => _lowStockCount;
        set => SetProperty(ref _lowStockCount, value);
    }

    public int OutOfStockCount
    {
        get => _outOfStockCount;
        set => SetProperty(ref _outOfStockCount, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand AdjustStockCommand { get; }

    public InventoryViewModel(ProductStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
        RefreshCommand = new Command(async () => await LoadInventoryAsync());
        AdjustStockCommand = new Command<ProductStock>(async (stock) => await OnAdjustStock(stock));

        _ = LoadInventoryAsync();
    }

    private async Task LoadInventoryAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            var stocks = await _stockRepository.GetAllWithProductsAsync();
            
            Stocks.Clear();
            decimal valuation = 0;
            int lowStock = 0;
            int outOfStock = 0;

            foreach (var stock in stocks)
            {
                if (string.IsNullOrWhiteSpace(SearchQuery) || 
                    stock.Product.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    stock.Product.Code.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                {
                    Stocks.Add(stock);
                }

                valuation += stock.BaseQuantity * stock.Product.CostPrice;
                if (stock.BaseQuantity <= 0) outOfStock++;
                else if (stock.BaseQuantity <= stock.ReorderLevel) lowStock++;
            }

            TotalValuation = valuation;
            LowStockCount = lowStock;
            OutOfStockCount = outOfStock;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading inventory: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnAdjustStock(ProductStock stock)
    {
        if (stock == null) return;
        // Navigation to stock adjustment or show a popup
        await Shell.Current.DisplayAlertAsync("Adjust Stock", $"Adjusting stock for {stock.Product.Name}", "OK");
    }
}
