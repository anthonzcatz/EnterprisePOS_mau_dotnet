using System.Collections.ObjectModel;
using EnterprisePOS.Helpers;
using EnterprisePOS.Interfaces;
using EnterprisePOS.Models;

namespace EnterprisePOS.ViewModels;

public sealed class DashboardViewModel : BaseViewModel
{
	private readonly IDashboardService dashboardService;

	public DashboardViewModel(IDashboardService dashboardService)
	{
		this.dashboardService = dashboardService;
		RefreshCommand = new Command(async () => await LoadAsync(), () => !IsBusy);
	}

	public ObservableCollection<DashboardMetric> Metrics { get; } = [];

	public Command RefreshCommand { get; }

	public async Task InitializeAsync()
	{
		if (Metrics.Count > 0)
		{
			return;
		}

		await LoadAsync();
	}

	private async Task LoadAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;
			RefreshCommand.ChangeCanExecute();
			Metrics.Clear();
			var metrics = await dashboardService.GetMetricsAsync();
			foreach (var metric in metrics)
			{
				Metrics.Add(metric);
			}
		}
		finally
		{
			IsBusy = false;
			RefreshCommand.ChangeCanExecute();
		}
	}
}
