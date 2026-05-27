using System.Windows.Input;

namespace EnterprisePOS.Components;

public partial class PosProductCardView : ContentView
{
	public static readonly BindableProperty TitleProperty =
		BindableProperty.Create(nameof(Title), typeof(string), typeof(PosProductCardView), string.Empty);

	public static readonly BindableProperty DescriptionProperty =
		BindableProperty.Create(nameof(Description), typeof(string), typeof(PosProductCardView), string.Empty);

	public static readonly BindableProperty PriceTextProperty =
		BindableProperty.Create(nameof(PriceText), typeof(string), typeof(PosProductCardView), string.Empty);

	public static readonly BindableProperty StockTextProperty =
		BindableProperty.Create(nameof(StockText), typeof(string), typeof(PosProductCardView), string.Empty);

	public static readonly BindableProperty StockBadgeTextProperty =
		BindableProperty.Create(nameof(StockBadgeText), typeof(string), typeof(PosProductCardView), string.Empty);

	public static readonly BindableProperty ImageSourceProperty =
		BindableProperty.Create(nameof(ImageSource), typeof(string), typeof(PosProductCardView), "dotnet_bot.png");

	public static readonly BindableProperty AddCommandProperty =
		BindableProperty.Create(nameof(AddCommand), typeof(ICommand), typeof(PosProductCardView));

	public static readonly BindableProperty CommandParameterProperty =
		BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(PosProductCardView));

	public PosProductCardView()
	{
		InitializeComponent();
	}

	public string Title
	{
		get => (string)GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public string Description
	{
		get => (string)GetValue(DescriptionProperty);
		set => SetValue(DescriptionProperty, value);
	}

	public string PriceText
	{
		get => (string)GetValue(PriceTextProperty);
		set => SetValue(PriceTextProperty, value);
	}

	public string StockText
	{
		get => (string)GetValue(StockTextProperty);
		set => SetValue(StockTextProperty, value);
	}

	public string StockBadgeText
	{
		get => (string)GetValue(StockBadgeTextProperty);
		set => SetValue(StockBadgeTextProperty, value);
	}

	public string ImageSource
	{
		get => (string)GetValue(ImageSourceProperty);
		set => SetValue(ImageSourceProperty, value);
	}

	public ICommand? AddCommand
	{
		get => (ICommand?)GetValue(AddCommandProperty);
		set => SetValue(AddCommandProperty, value);
	}

	public object? CommandParameter
	{
		get => GetValue(CommandParameterProperty);
		set => SetValue(CommandParameterProperty, value);
	}
}
