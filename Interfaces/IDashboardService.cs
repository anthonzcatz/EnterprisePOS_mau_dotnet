using EnterprisePOS.Features.Dashboard.Models;

namespace EnterprisePOS.Interfaces;

public interface IDashboardService
{
	Task<IReadOnlyList<DashboardMetric>> GetMetricsAsync(CancellationToken cancellationToken = default);
}
