using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using EnterprisePOS.Core.Data.Models;
using EnterprisePOS.Core.Data.Repositories;
using EnterprisePOS.Helpers;

namespace EnterprisePOS.Features.Customers.ViewModels;

public class CustomersViewModel : BaseViewModel
{
    private readonly CustomerRepository _customerRepository;
    private string _searchQuery = string.Empty;

    public ObservableCollection<Customer> Customers { get; } = new();

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
            {
                _ = LoadCustomersAsync();
            }
        }
    }

    public ICommand RefreshCommand { get; }
    public ICommand AddCustomerCommand { get; }
    public ICommand ViewDetailsCommand { get; }

    public CustomersViewModel(CustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
        RefreshCommand = new Command(async () => await LoadCustomersAsync());
        AddCustomerCommand = new Command(async () => await OnAddCustomer());
        ViewDetailsCommand = new Command<Customer>(async (customer) => await OnViewDetails(customer));
    }

    private async Task LoadCustomersAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            var customers = await _customerRepository.GetAllAsync();
            
            Customers.Clear();
            foreach (var customer in customers)
            {
                if (string.IsNullOrWhiteSpace(SearchQuery) || 
                    customer.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    customer.Code.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    (customer.Phone?.Contains(SearchQuery) ?? false))
                {
                    Customers.Add(customer);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading customers: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnAddCustomer()
    {
        await Shell.Current.GoToAsync("CustomerDetailsPage");
    }

    private async Task OnViewDetails(Customer customer)
    {
        if (customer == null) return;

        await Shell.Current.GoToAsync("CustomerDetailsPage", new Dictionary<string, object>
        {
            { "Customer", customer }
        });
    }
}
