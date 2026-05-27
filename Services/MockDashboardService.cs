using EnterprisePOS.Interfaces;
using EnterprisePOS.Models;

namespace EnterprisePOS.Services;

public sealed class MockDashboardService : IDashboardService
{
	public Task<IReadOnlyList<DashboardMetric>> GetMetricsAsync(CancellationToken cancellationToken = default)
	{
		IReadOnlyList<DashboardMetric> metrics =
		[
			new DashboardMetric { Title = "Today Sales", Value = "PHP 58,240", Delta = "+12.4%" },
			new DashboardMetric { Title = "Orders", Value = "143", Delta = "+8.1%" },
			new DashboardMetric { Title = "Low Stock Items", Value = "17", Delta = "-3 today" },
			new DashboardMetric { Title = "Bookings", Value = "28", Delta = "+4.2%" }
		];

		return Task.FromResult(metrics);
	}
}
