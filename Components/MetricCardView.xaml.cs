namespace EnterprisePOS.Components;

public partial class MetricCardView : ContentView
{
	public static readonly BindableProperty IconProperty = BindableProperty.Create(
		nameof(Icon),
		typeof(string),
		typeof(MetricCardView),
		"•");

	public static readonly BindableProperty TitleProperty = BindableProperty.Create(
		nameof(Title),
		typeof(string),
		typeof(MetricCardView),
		string.Empty);

	public static readonly BindableProperty ValueProperty = BindableProperty.Create(
		nameof(Value),
		typeof(string),
		typeof(MetricCardView),
		string.Empty);

	public static readonly BindableProperty DeltaProperty = BindableProperty.Create(
		nameof(Delta),
		typeof(string),
		typeof(MetricCardView),
		string.Empty);

	public static readonly BindableProperty IsPositiveProperty = BindableProperty.Create(
		nameof(IsPositive),
		typeof(bool),
		typeof(MetricCardView),
		true);

	public MetricCardView()
	{
		InitializeComponent();
	}

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

	public string Value
	{
		get => (string)GetValue(ValueProperty);
		set => SetValue(ValueProperty, value);
	}

	public string Delta
	{
		get => (string)GetValue(DeltaProperty);
		set => SetValue(DeltaProperty, value);
	}

	public bool IsPositive
	{
		get => (bool)GetValue(IsPositiveProperty);
		set => SetValue(IsPositiveProperty, value);
	}
}
