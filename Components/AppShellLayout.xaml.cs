using EnterprisePOS.Helpers;
using EnterprisePOS.ViewModels;

namespace EnterprisePOS.Components;

public partial class AppShellLayout : Grid
{
	public static readonly BindableProperty ActiveKeyProperty =
		BindableProperty.Create(nameof(ActiveKey), typeof(string), typeof(AppShellLayout), string.Empty,
			propertyChanged: (b, _, n) => ((AppShellLayout)b).ShellVm.SetActiveRoute((string)(n ?? string.Empty)));

	public static readonly BindableProperty PageTitleProperty =
		BindableProperty.Create(nameof(PageTitle), typeof(string), typeof(AppShellLayout), string.Empty,
			propertyChanged: (b, _, n) =>
			{
				var layout = (AppShellLayout)b;
				if (layout.NavbarTitle != null)
					layout.NavbarTitle.Text = (string)(n ?? string.Empty);
			});

	public static readonly BindableProperty PageBodyProperty =
		BindableProperty.Create(nameof(PageBody), typeof(View), typeof(AppShellLayout), null,
			propertyChanged: (b, _, n) =>
			{
				var layout = (AppShellLayout)b;
				if (layout.PageContent != null)
				{
					layout.PageContent.Content = (View?)n;
					layout.UpdatePageBodyBindingContext();
				}
			});

	public AppShellLayout()
	{
		ShellVm = ServiceHelper.GetRequiredService<AppShellViewModel>();
		InitializeComponent();

		NavbarTitle.Text = PageTitle;
		PageContent.Content = PageBody;
		UpdatePageBodyBindingContext();

		// Initial responsive setup - default to sidebar visible on desktop
		DesktopSidebar.IsVisible = true;
		SidebarColumn.Width = new GridLength(ShellVm.IsSidebarCollapsed ? 76 : 240);

		SizeChanged += (_, _) => UpdateResponsive(Width);
		ShellVm.PropertyChanged += (_, e) =>
		{
			if (e.PropertyName is nameof(ShellVm.IsSidebarCollapsed) or nameof(ShellVm.ShowSidebar))
				UpdateResponsive(Width);
		};
	}

	public AppShellViewModel ShellVm { get; }

	public string ActiveKey
	{
		get => (string)GetValue(ActiveKeyProperty);
		set => SetValue(ActiveKeyProperty, value);
	}

	public string PageTitle
	{
		get => (string)GetValue(PageTitleProperty);
		set => SetValue(PageTitleProperty, value);
	}

	public View? PageBody
	{
		get => (View?)GetValue(PageBodyProperty);
		set => SetValue(PageBodyProperty, value);
	}

	protected override void OnBindingContextChanged()
	{
		base.OnBindingContextChanged();
		UpdatePageBodyBindingContext();
	}

	private void UpdatePageBodyBindingContext()
	{
		if (PageContent?.Content != null)
		{
			PageContent.Content.BindingContext = BindingContext;
		}
	}

	private void UpdateResponsive(double width)
	{
		if (width <= 0) return;
		ShellVm.UpdateAvailableWidth(width);
		var showSidebar = ShellVm.ShowSidebar;
		DesktopSidebar.IsVisible = showSidebar;
		MobileNavbar.IsVisible = !showSidebar;
		SidebarColumn.Width = showSidebar
			? new GridLength(ShellVm.IsSidebarCollapsed ? 76 : 240)
			: new GridLength(0);
	}
}
