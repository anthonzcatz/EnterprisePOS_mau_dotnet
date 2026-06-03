using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using EnterprisePOS.Core.Data.Models;
using EnterprisePOS.Core.Data.Repositories;
using EnterprisePOS.Helpers;

namespace EnterprisePOS.Features.Sales.ViewModels;

public class SalesViewModel : BaseViewModel
{
    private readonly SaleRepository _saleRepository;
    private string _searchQuery = string.Empty;
    private decimal _totalSales;
    private int _transactionCount;

    public ObservableCollection<Sale> Sales { get; } = new();

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
            {
                _ = LoadSalesAsync();
            }
        }
    }

    public decimal TotalSales
    {
        get => _totalSales;
        set => SetProperty(ref _totalSales, value);
    }

    public int TransactionCount
    {
        get => _transactionCount;
        set => SetProperty(ref _transactionCount, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand ViewDetailsCommand { get; }

    public SalesViewModel(SaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
        RefreshCommand = new Command(async () => await LoadSalesAsync());
        ViewDetailsCommand = new Command<Sale>(async (sale) => await OnViewDetails(sale));

        _ = LoadSalesAsync();
    }

    private async Task LoadSalesAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            var sales = await _saleRepository.GetAllAsync();
            
            Sales.Clear();
            decimal total = 0;
            foreach (var sale in sales)
            {
                if (string.IsNullOrWhiteSpace(SearchQuery) || 
                    sale.SaleNumber.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    (sale.Customer?.Name?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    Sales.Add(sale);
                    total += sale.TotalAmount;
                }
            }

            TotalSales = total;
            TransactionCount = Sales.Count;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading sales: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnViewDetails(Sale sale)
    {
        if (sale == null) return;
        await Shell.Current.DisplayAlertAsync("Sale Details", $"Details for {sale.SaleNumber}", "OK");
    }
}
