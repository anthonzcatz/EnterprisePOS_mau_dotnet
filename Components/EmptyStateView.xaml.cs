using System.Windows.Input;

namespace EnterprisePOS.Components;

public partial class EmptyStateView : ContentView
{
	public static readonly BindableProperty IconProperty =
		BindableProperty.Create(nameof(Icon), typeof(string), typeof(EmptyStateView), "📦");

	public static readonly BindableProperty TitleProperty =
		BindableProperty.Create(nameof(Title), typeof(string), typeof(EmptyStateView), "No Data");

	public static readonly BindableProperty MessageProperty =
		BindableProperty.Create(nameof(Message), typeof(string), typeof(EmptyStateView), "There's nothing to show here.");

	public static readonly BindableProperty ActionTextProperty =
		BindableProperty.Create(nameof(ActionText), typeof(string), typeof(EmptyStateView), string.Empty);

	public static readonly BindableProperty ActionCommandProperty =
		BindableProperty.Create(nameof(ActionCommand), typeof(ICommand), typeof(EmptyStateView), null);

	public string Icon
	{
		get => (string)GetValue(IconProperty);
		set => SetValue(IconProperty, value);
	}

	public string Title
	{
		get => (string)GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public string Message
	{
		get => (string)GetValue(MessageProperty);
		set => SetValue(MessageProperty, value);
	}

	public string ActionText
	{
		get => (string)GetValue(ActionTextProperty);
		set => SetValue(ActionTextProperty, value);
	}

	public ICommand ActionCommand
	{
		get => (ICommand)GetValue(ActionCommandProperty);
		set => SetValue(ActionCommandProperty, value);
	}

	public bool HasAction => !string.IsNullOrEmpty(ActionText) && ActionCommand != null;

	public EmptyStateView()
	{
		InitializeComponent();
	}
}
