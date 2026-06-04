namespace EnterprisePOS.Helpers;

/// <summary>Width thresholds (logical pixels) for responsive shell and page layouts.</summary>
public static class LayoutBreakpoints
{
	/// <summary>Phone / narrow — bottom TabBar, flyout hidden.</summary>
	public const double MobileMax = 599;

	/// <summary>Landscape tablet / large tablet width that should use laptop-style layout.</summary>
	public const double LargeTabletLandscapeMin = 768;

	/// <summary>Tablet / small laptop — collapsible flyout, POS 2-column.</summary>
	public const double TabletMax = 899;

	/// <summary>Desktop — locked flyout, POS 3-column wide layout.</summary>
	public const double DesktopMin = 900;

	/// <summary>Minimum width for the persistent sidebar to be visible.</summary>
	public const double SidebarVisibleMin = 1000;
}
