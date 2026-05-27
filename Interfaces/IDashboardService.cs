using EnterprisePOS.Models;

namespace EnterprisePOS.Interfaces;

public interface IDashboardService
{
	Task<IReadOnlyList<DashboardMetric>> GetMetricsAsync(CancellationToken cancellationToken = default);
}
