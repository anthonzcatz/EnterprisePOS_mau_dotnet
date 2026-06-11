using System.Collections.ObjectModel;
using System.Windows.Input;
using EnterprisePOS.Core.Data.Models;
using EnterprisePOS.Core.Data.Repositories;
using EnterprisePOS.Helpers;

namespace EnterprisePOS.Features.Units.ViewModels;

public class UnitListViewModel : BaseViewModel
{
    private readonly UnitRepository _unitRepository;

    public ObservableCollection<Unit> Units { get; } = new();

    public ICommand AddUnitCommand { get; }
    public ICommand RefreshCommand { get; }

    public UnitListViewModel(UnitRepository unitRepository)
    {
        _unitRepository = unitRepository;
        AddUnitCommand = new Command(async () => await AddUnitAsync());
        RefreshCommand = new Command(async () => await LoadUnitsAsync());
        
        _ = LoadUnitsAsync();
    }

    private async Task LoadUnitsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var units = await _unitRepository.GetAllAsync();
            Units.Clear();
            foreach (var unit in units) Units.Add(unit);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddUnitAsync()
    {
        // For units, we could have a simple dialog or a separate page.
        // For now, let's just show how we'd handle it.
        await Task.CompletedTask;
    }
}
