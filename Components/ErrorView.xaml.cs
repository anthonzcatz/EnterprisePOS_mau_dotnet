using System.Windows.Input;

namespace EnterprisePOS.Components;

public partial class ErrorView : ContentView
{
	public static readonly BindableProperty TitleProperty =
		BindableProperty.Create(nameof(Title), typeof(string), typeof(ErrorView), "Error");

	public static readonly BindableProperty MessageProperty =
		BindableProperty.Create(nameof(Message), typeof(string), typeof(ErrorView), "Something went wrong.");

	public static readonly BindableProperty RetryCommandProperty =
		BindableProperty.Create(nameof(RetryCommand), typeof(ICommand), typeof(ErrorView), null);

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

	public ICommand RetryCommand
	{
		get => (ICommand)GetValue(RetryCommandProperty);
		set => SetValue(RetryCommandProperty, value);
	}

	public bool HasRetry => RetryCommand != null;

	public ErrorView()
	{
		InitializeComponent();
	}
}
