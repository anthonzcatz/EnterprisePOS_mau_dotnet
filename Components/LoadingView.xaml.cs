namespace EnterprisePOS.Components;

public partial class LoadingView : ContentView
{
	public static readonly BindableProperty MessageProperty =
		BindableProperty.Create(nameof(Message), typeof(string), typeof(LoadingView), "Loading...");

	public string Message
	{
		get => (string)GetValue(MessageProperty);
		set => SetValue(MessageProperty, value);
	}

	public LoadingView()
	{
		InitializeComponent();
	}
}
