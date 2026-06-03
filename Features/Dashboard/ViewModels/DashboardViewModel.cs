using System.Collections.ObjectModel;
using EnterprisePOS.Helpers;
using EnterprisePOS.Interfaces;
using EnterprisePOS.Features.Dashboard.Models;
namespace EnterprisePOS.Features.Dashboard.ViewModels;

public sealed class DashboardViewModel : BaseViewModel
{
	private readonly IDashboardService dashboardService;

	public DashboardViewModel(IDashboardService dashboardService)
	{
		this.dashboardService = dashboardService;
		RefreshCommand = new Command(async () => await LoadAsync(), () => !IsBusy);
		
		// Initialize with sample data
		LoadSampleData();
	}

	public ObservableCollection<DashboardMetric> Metrics { get; } = [];
	public ObservableCollection<RecentActivity> RecentActivities { get; } = [];

	public Command RefreshCommand { get; }

	public string CurrentDate => DateTime.Now.ToString("MMM dd, yyyy");
	public string TodaySales => "₱12,450";
	public string SalesGrowth => "+15%";
	public string OrderCount => "48";
	public string Revenue => "₱15,890";
	public string CustomerCount => "23";
	public string LowStockCount => "5";
	public string PendingOrders => "3";
	public string ActiveTerminals => "4";
	public string TotalProducts => "156";

	public async Task InitializeAsync()
	{
		if (Metrics.Count > 0)
		{
			return;
		}

		await LoadAsync();
	}
	private void LoadSampleData()
	{
		RecentActivities.Clear();
		RecentActivities.Add(new RecentActivity { Initial = "JD", Title = "New Sale", Description = "Order #1234 completed", Time = "2 min ago" });
		RecentActivities.Add(new RecentActivity { Initial = "MK", Title = "Product Added", Description = "New product: Coffee", Time = "15 min ago" });
		RecentActivities.Add(new RecentActivity { Initial = "AS", Title = "Customer", Description = "New customer registered", Time = "1 hour ago" });
		RecentActivities.Add(new RecentActivity { Initial = "RB", Title = "Stock Alert", Description = "Low stock: Burger", Time = "2 hours ago" });
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

public class RecentActivity
{
	public string Initial { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Time { get; set; } = string.Empty;
}
