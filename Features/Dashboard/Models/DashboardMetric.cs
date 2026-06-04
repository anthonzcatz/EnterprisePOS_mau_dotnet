using System.Windows.Input;

namespace EnterprisePOS.Features.Dashboard.Models;

public class DashboardMetric
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Delta { get; set; } = string.Empty;
    public bool IsPositive { get; set; } = true;
    public ICommand? Command { get; set; }
}
