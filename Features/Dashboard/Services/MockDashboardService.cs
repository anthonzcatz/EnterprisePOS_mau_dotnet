using EnterprisePOS.Interfaces;
using EnterprisePOS.Features.Dashboard.Models;

namespace EnterprisePOS.Features.Dashboard.Services;

public sealed class MockDashboardService : IDashboardService
{
	public Task<IReadOnlyList<DashboardMetric>> GetMetricsAsync(CancellationToken cancellationToken = default)
	{
		IReadOnlyList<DashboardMetric> metrics =
		[
			new() { Title = "Gross Sales", Value = "PHP 58,240", Delta = "+12.4% vs yesterday", Icon = "₱", IsPositive = true },
			new() { Title = "Orders", Value = "143", Delta = "+8.1% processed", Icon = "🧾", IsPositive = true },
			new() { Title = "Low Stock Items", Value = "17", Delta = "-3 resolved today", Icon = "⚠", IsPositive = false },
			new() { Title = "Bookings", Value = "28", Delta = "+4 walk-ins converted", Icon = "📅", IsPositive = true },
			new() { Title = "Receivables", Value = "PHP 12,800", Delta = "6 accounts due", Icon = "⌁", IsPositive = false },
			new() { Title = "Active Terminals", Value = "4 / 5", Delta = "1 terminal idle", Icon = "⌘", IsPositive = true }
		];

		return Task.FromResult(metrics);
	}
}
