using System.Collections.ObjectModel;
using EnterprisePOS.Helpers;
using EnterprisePOS.Interfaces;
using EnterprisePOS.Features.Dashboard.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Measure;
using SkiaSharp;
using LiveChartsCore.Drawing;

namespace EnterprisePOS.Features.Dashboard.ViewModels;

public sealed class DashboardViewModel : BaseViewModel
{
	private readonly IDashboardService dashboardService;

	public DashboardViewModel(IDashboardService dashboardService)
	{
		this.dashboardService = dashboardService;
		RefreshCommand = new Command(async () => await LoadAsync(), () => !IsBusy);
		
		// Initialize navigation commands
		NavigateToPOSCommand = new Command(async () => await Shell.Current.GoToAsync("//pos/pos-main"));
		NavigateToSalesCommand = new Command(async () => await Shell.Current.GoToAsync("//sales/sales-main"));
		NavigateToProductsCommand = new Command(async () => await Shell.Current.GoToAsync("//products/products-main"));
		NavigateToInventoryCommand = new Command(async () => await Shell.Current.GoToAsync("//inventory/inventory-main"));
		NavigateToCustomersCommand = new Command(async () => await Shell.Current.GoToAsync("//customers/customers-main"));
		NavigateToReportsCommand = new Command(async () => await Shell.Current.GoToAsync("//reports/reports-main"));
		
		// Initialize with sample data
		LoadSampleData();
		InitializeCharts();
	}

	public ObservableCollection<DashboardMetric> Metrics { get; } = [];
	public ObservableCollection<RecentActivity> RecentActivities { get; } = [];
	
	// LiveCharts Series Collections
	public IEnumerable<ISeries> SalesTrendSeries { get; private set; } = [];
	public IEnumerable<ISeries> PaymentMixSeries { get; private set; } = [];
	public IEnumerable<ISeries> BranchSalesSeries { get; private set; } = [];
	public IEnumerable<ISeries> TopProductsSeries { get; private set; } = [];
	public IEnumerable<ISeries> CategoryMixSeries { get; private set; } = [];
	
	// X-Axes for charts
	public Axis[] SalesTrendXAxes { get; private set; } = [];
	public Axis[] BranchSalesXAxes { get; private set; } = [];
	public Axis[] TopProductsXAxes { get; private set; } = [];
	public Axis[] CategoryMixXAxes { get; private set; } = [];

	public Command RefreshCommand { get; }

	// Navigation Commands
	public Command NavigateToPOSCommand { get; }
	public Command NavigateToSalesCommand { get; }
	public Command NavigateToProductsCommand { get; }
	public Command NavigateToInventoryCommand { get; }
	public Command NavigateToCustomersCommand { get; }
	public Command NavigateToReportsCommand { get; }

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

	private void InitializeCharts()
	{
		InitializeSalesTrendChart();
		InitializePaymentMixChart();
		InitializeBranchSalesChart();
		InitializeTopProductsChart();
		InitializeCategoryMixChart();
	}

	private void InitializeSalesTrendChart()
	{
		var colors = new SKColor[] 
		{
			SKColor.Parse("#356AE6"), // Monday (weekday)
			SKColor.Parse("#356AE6"), // Tuesday (weekday)
			SKColor.Parse("#356AE6"), // Wednesday (weekday)
			SKColor.Parse("#1FA66A"), // Thursday (weekend prep)
			SKColor.Parse("#1FA66A"), // Friday (weekend peak)
			SKColor.Parse("#1FA66A"), // Saturday (weekend)
			SKColor.Parse("#9CA3AF")  // Sunday (closed/slow)
		};

		SalesTrendSeries = new ISeries[]
		{
			new ColumnSeries<double>
			{
				Values = new double[] { 25000, 19000, 28000, 22000, 31000, 33000, 17000 },
				Fill = new SolidColorPaint(SKColor.Parse("#356AE6")),
				DataLabelsPosition = DataLabelsPosition.Top,
				DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#1F2A3D")),
				DataLabelsSize = 12,
				DataLabelsFormatter = p => $"₱{p.Coordinate.PrimaryValue/1000:F0}k"
			}
		};

		SalesTrendXAxes = new[]
		{
			new Axis
			{
				Labels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" },
				LabelsPaint = new SolidColorPaint(SKColor.Parse("#73819A")),
				SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E7EB")) { StrokeThickness = 1 }
			}
		};
	}

	private void InitializePaymentMixChart()
	{
		PaymentMixSeries = new ISeries[]
		{
			new PieSeries<double> { Values = new double[] { 45 }, Fill = new SolidColorPaint(SKColor.Parse("#356AE6")), Name = "Cash" },
			new PieSeries<double> { Values = new double[] { 35 }, Fill = new SolidColorPaint(SKColor.Parse("#1FA66A")), Name = "Card" },
			new PieSeries<double> { Values = new double[] { 20 }, Fill = new SolidColorPaint(SKColor.Parse("#FCD34D")), Name = "Digital" }
		};
	}

	private void InitializeBranchSalesChart()
	{
		BranchSalesSeries = new ISeries[]
		{
			new ColumnSeries<double>
			{
				Values = new double[] { 85000, 52000, 68000 },
				Fill = new SolidColorPaint(SKColor.Parse("#356AE6")),
				DataLabelsPosition = DataLabelsPosition.Top,
				DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#1F2A3D")),
				DataLabelsSize = 11,
				DataLabelsFormatter = p => $"₱{p.Coordinate.PrimaryValue/1000:F0}k"
			}
		};

		BranchSalesXAxes = new[]
		{
			new Axis
			{
				Labels = new[] { "Main", "North", "South" },
				LabelsPaint = new SolidColorPaint(SKColor.Parse("#73819A")),
				SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E7EB")) { StrokeThickness = 1 }
			}
		};
	}

	private void InitializeTopProductsChart()
	{
		TopProductsSeries = new ISeries[]
		{
			new RowSeries<double>
			{
				Values = new double[] { 450, 380, 320 },
				Fill = new SolidColorPaint(SKColor.Parse("#356AE6")),
				DataLabelsPosition = DataLabelsPosition.Right,
				DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#1F2A3D")),
				DataLabelsSize = 11,
				DataLabelsFormatter = p => $"{p.Coordinate.PrimaryValue:F0} sold"
			}
		};

		TopProductsXAxes = new[]
		{
			new Axis
			{
				Labels = new[] { "☕ Americano", "🥐 Croissant", "🍰 Cake Slice" },
				LabelsPaint = new SolidColorPaint(SKColor.Parse("#73819A")),
				Position = AxisPosition.Start
			}
		};
	}

	private void InitializeCategoryMixChart()
	{
		CategoryMixSeries = new ISeries[]
		{
			new ColumnSeries<double>
			{
				Values = new double[] { 45000, 30000, 25000 },
				Fill = new SolidColorPaint(SKColor.Parse("#356AE6")),
				DataLabelsPosition = DataLabelsPosition.Top,
				DataLabelsPaint = new SolidColorPaint(SKColor.Parse("#1F2A3D")),
				DataLabelsSize = 11,
				DataLabelsFormatter = p => $"₱{p.Coordinate.PrimaryValue/1000:F0}k"
			}
		};

		CategoryMixXAxes = new[]
		{
			new Axis
			{
				Labels = new[] { "Beverages", "Pastries", "Meals" },
				LabelsPaint = new SolidColorPaint(SKColor.Parse("#73819A")),
				SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E7EB")) { StrokeThickness = 1 }
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

		// Add metrics with navigation
		Metrics.Clear();
		Metrics.Add(new DashboardMetric { Title = "Today's Sales", Value = TodaySales, Delta = SalesGrowth, Icon = "₱", IsPositive = true, Command = NavigateToPOSCommand });
		Metrics.Add(new DashboardMetric { Title = "Total Orders", Value = OrderCount, Delta = "Orders", Icon = "📦", IsPositive = true, Command = NavigateToSalesCommand });
		Metrics.Add(new DashboardMetric { Title = "Revenue", Value = Revenue, Delta = "Daily", Icon = "💰", IsPositive = true, Command = NavigateToPOSCommand });
		Metrics.Add(new DashboardMetric { Title = "Customers", Value = CustomerCount, Delta = "Active", Icon = "👥", IsPositive = true, Command = NavigateToCustomersCommand });
		Metrics.Add(new DashboardMetric { Title = "Low Stock", Value = LowStockCount, Delta = "Items", Icon = "⚠️", IsPositive = false, Command = NavigateToInventoryCommand });
		Metrics.Add(new DashboardMetric { Title = "Pending Orders", Value = PendingOrders, Delta = "Orders", Icon = "⏳", IsPositive = false, Command = NavigateToSalesCommand });
		Metrics.Add(new DashboardMetric { Title = "Active Terminals", Value = ActiveTerminals, Delta = "Online", Icon = "🖥️", IsPositive = true, Command = NavigateToPOSCommand });
		Metrics.Add(new DashboardMetric { Title = "Total Products", Value = TotalProducts, Delta = "Items", Icon = "📦", IsPositive = true, Command = NavigateToProductsCommand });
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
