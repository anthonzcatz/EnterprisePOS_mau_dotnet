using EnterprisePOS.Helpers;
using EnterprisePOS.ViewModels;

namespace EnterprisePOS.Components;

public partial class AppShellLayout : ContentView
{
	public static readonly BindableProperty ActiveKeyProperty =
		BindableProperty.Create(nameof(ActiveKey), typeof(string), typeof(AppShellLayout), string.Empty,
			propertyChanged: (b, _, n) => ((AppShellLayout)b).ShellVm.SetActiveRoute((string)(n ?? string.Empty)));

	public AppShellLayout()
	{
		ShellVm = ServiceHelper.GetRequiredService<AppShellViewModel>();
		BindingContext = this;
		InitializeComponent();
		SizeChanged += OnSizeChanged;
	}

	public AppShellViewModel ShellVm { get; }

	public string ActiveKey
	{
		get => (string)GetValue(ActiveKeyProperty);
		set => SetValue(ActiveKeyProperty, value);
	}

	private void OnSizeChanged(object? sender, EventArgs e)
	{
		// Get the page width from the parent
		var page = this.GetParentOfType<ContentPage>();
		if (page is not null)
		{
			ShellVm.UpdateAvailableWidth(page.Width);
		}
	}

	protected override void OnParentSet()
	{
		base.OnParentSet();
		// Initial width update
		var page = this.GetParentOfType<ContentPage>();
		if (page is not null)
		{
			page.SizeChanged += (s, e) => ShellVm.UpdateAvailableWidth(page.Width);
		}
	}
}

internal static class ViewExtensions
{
	public static T? GetParentOfType<T>(this Element element) where T : Element
	{
		var parent = element.Parent;
		while (parent is not null)
		{
			if (parent is T typed) return typed;
			parent = parent.Parent;
		}
		return null;
	}
}
