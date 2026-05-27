namespace EnterprisePOS.Helpers;

/// <summary>Width thresholds (logical pixels) for responsive shell and page layouts.</summary>
public static class LayoutBreakpoints
{
	/// <summary>Phone / narrow — bottom TabBar, flyout hidden.</summary>
	public const double MobileMax = 599;

	/// <summary>Tablet / small laptop — collapsible flyout, POS 2-column.</summary>
	public const double TabletMax = 899;

	/// <summary>Desktop — locked flyout, POS 3-column wide layout.</summary>
	public const double DesktopMin = 900;
}
