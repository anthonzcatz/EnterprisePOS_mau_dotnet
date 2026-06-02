using System.Collections.ObjectModel;
using EnterprisePOS.Helpers;
using EnterprisePOS.Interfaces;
using EnterprisePOS.Features.Dashboard.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LvcPainting = LiveChartsCore.SkiaSharpView.Painting;

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
		InitializeCharts();
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

	// LiveChartsCore Series
	private ISeries[] salesChartSeries = [];
	public ISeries[] SalesChartSeries
	{
		get => salesChartSeries;
		private set
		{
			salesChartSeries = value;
			OnPropertyChanged();
		}
	}

	private ISeries[] paymentMethodSeries = [];
	public ISeries[] PaymentMethodSeries
	{
		get => paymentMethodSeries;
		private set
		{
			paymentMethodSeries = value;
			OnPropertyChanged();
		}
	}

	private ISeries[] productCategorySeries = [];
	public ISeries[] ProductCategorySeries
	{
		get => productCategorySeries;
		private set
		{
			productCategorySeries = value;
			OnPropertyChanged();
		}
	}

	private ISeries[] salesByBranchSeries = [];
	public ISeries[] SalesByBranchSeries
	{
		get => salesByBranchSeries;
		private set
		{
			salesByBranchSeries = value;
			OnPropertyChanged();
		}
	}

	private ISeries[] topProductsSeries = [];
	public ISeries[] TopProductsSeries
	{
		get => topProductsSeries;
		private set
		{
			topProductsSeries = value;
			OnPropertyChanged();
		}
	}

	public async Task InitializeAsync()
	{
		if (Metrics.Count > 0)
		{
			return;
		}

		await LoadAsync();
	}

	private void InitializeCharts()
	{
		// Sales Trend Chart - Line Chart with gradient
		var salesGradient = new LvcPainting.LinearGradientPaint(
			new[] { new SKColor(44, 123, 229), new SKColor(0, 217, 126) },
			new SKPoint(0, 1),
			new SKPoint(0, 0));

		SalesChartSeries = new ISeries[]
		{
			new LineSeries<int>
			{
				Values = new[] { 60, 90, 50, 120, 80, 100, 70 },
				Name = "Sales",
				Fill = salesGradient,
				Stroke = new LvcPainting.SolidColorPaint(new SKColor(44, 123, 229)) { StrokeThickness = 4 },
				GeometrySize = 10,
				GeometryStroke = new LvcPainting.SolidColorPaint(new SKColor(44, 123, 229)) { StrokeThickness = 2 }
			}
		};

		// Payment Methods Chart - Doughnut with Gradient
		var cashGradient = new LvcPainting.LinearGradientPaint(
			new[] { new SKColor(44, 123, 229), new SKColor(74, 144, 226) },
			new SKPoint(0, 0),
			new SKPoint(1, 1));

		var cardGradient = new LvcPainting.LinearGradientPaint(
			new[] { new SKColor(0, 217, 126), new SKColor(16, 185, 129) },
			new SKPoint(0, 0),
			new SKPoint(1, 1));

		var gcashGradient = new LvcPainting.LinearGradientPaint(
			new[] { new SKColor(246, 195, 67), new SKColor(245, 158, 11) },
			new SKPoint(0, 0),
			new SKPoint(1, 1));

		PaymentMethodSeries = new ISeries[]
		{
			new PieSeries<int> { Values = new[] { 45 }, Name = "Cash", Fill = cashGradient },
			new PieSeries<int> { Values = new[] { 35 }, Name = "Card", Fill = cardGradient },
			new PieSeries<int> { Values = new[] { 20 }, Name = "GCash", Fill = gcashGradient }
		};

		// Product Categories Chart - Horizontal Bar with Gradient
		var categoryGradient = new LvcPainting.LinearGradientPaint(
			new[] { new SKColor(139, 92, 246), new SKColor(59, 130, 246) },
			new SKPoint(0, 0),
			new SKPoint(1, 0));

		ProductCategorySeries = new ISeries[]
		{
			new RowSeries<int>
			{
				Values = new[] { 85, 65, 45, 30, 20 },
				Name = "Categories",
				Fill = categoryGradient,
				Stroke = new LvcPainting.SolidColorPaint(new SKColor(139, 92, 246)) { StrokeThickness = 2 }
			}
		};

		// Sales by Branch Chart - Column Chart
		var branchGradient = new LvcPainting.LinearGradientPaint(
			new[] { new SKColor(236, 72, 153), new SKColor(219, 39, 119) },
			new SKPoint(0, 1),
			new SKPoint(0, 0));

		SalesByBranchSeries = new ISeries[]
		{
			new ColumnSeries<int>
			{
				Values = new[] { 45, 30, 25 },
				Name = "Branch Sales",
				Fill = branchGradient,
				Stroke = new LvcPainting.SolidColorPaint(new SKColor(236, 72, 153)) { StrokeThickness = 2 }
			}
		};

		// Top Products Chart - Bar Chart
		var productGradient = new LvcPainting.LinearGradientPaint(
			new[] { new SKColor(16, 185, 129), new SKColor(5, 150, 105) },
			new SKPoint(0, 0),
			new SKPoint(1, 0));

		TopProductsSeries = new ISeries[]
		{
			new RowSeries<int>
			{
				Values = new[] { 120, 95, 80, 65, 50 },
				Name = "Top Products",
				Fill = productGradient,
				Stroke = new LvcPainting.SolidColorPaint(new SKColor(16, 185, 129)) { StrokeThickness = 2 }
			}
		};
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
