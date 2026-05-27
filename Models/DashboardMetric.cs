namespace EnterprisePOS.Models;

public sealed class DashboardMetric
{
	public string Title { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
	public string Delta { get; set; } = string.Empty;
}
