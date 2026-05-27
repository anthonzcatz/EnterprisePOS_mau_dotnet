using System.Windows.Input;

namespace EnterprisePOS.Components;

public partial class PosCartLineView : ContentView
{
	public static readonly BindableProperty TitleProperty =
		BindableProperty.Create(nameof(Title), typeof(string), typeof(PosCartLineView), string.Empty);

	public static readonly BindableProperty DescriptionProperty =
		BindableProperty.Create(nameof(Description), typeof(string), typeof(PosCartLineView), string.Empty);

	public static readonly BindableProperty PriceTextProperty =
		BindableProperty.Create(nameof(PriceText), typeof(string), typeof(PosCartLineView), string.Empty);

	public static readonly BindableProperty QuantityTextProperty =
		BindableProperty.Create(nameof(QuantityText), typeof(string), typeof(PosCartLineView), "1");

	public static readonly BindableProperty ImageSourceProperty =
		BindableProperty.Create(nameof(ImageSource), typeof(string), typeof(PosCartLineView), "dotnet_bot.png");

	public static readonly BindableProperty SizeProperty =
		BindableProperty.Create(nameof(Size), typeof(string), typeof(PosCartLineView), string.Empty);

	public static readonly BindableProperty ColorProperty =
		BindableProperty.Create(nameof(Color), typeof(string), typeof(PosCartLineView), string.Empty);

	public static readonly BindableProperty TotalTextProperty =
		BindableProperty.Create(nameof(TotalText), typeof(string), typeof(PosCartLineView), string.Empty);

	public static readonly BindableProperty IncreaseCommandProperty =
		BindableProperty.Create(nameof(IncreaseCommand), typeof(ICommand), typeof(PosCartLineView));

	public static readonly BindableProperty DecreaseCommandProperty =
		BindableProperty.Create(nameof(DecreaseCommand), typeof(ICommand), typeof(PosCartLineView));

	public static readonly BindableProperty DeleteCommandProperty =
		BindableProperty.Create(nameof(DeleteCommand), typeof(ICommand), typeof(PosCartLineView));

	public static readonly BindableProperty CommandParameterProperty =
		BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(PosCartLineView));

	public PosCartLineView()
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

	public string QuantityText
	{
		get => (string)GetValue(QuantityTextProperty);
		set => SetValue(QuantityTextProperty, value);
	}

	public string ImageSource
	{
		get => (string)GetValue(ImageSourceProperty);
		set => SetValue(ImageSourceProperty, value);
	}

	public string Size
	{
		get => (string)GetValue(SizeProperty);
		set => SetValue(SizeProperty, value);
	}

	public string Color
	{
		get => (string)GetValue(ColorProperty);
		set => SetValue(ColorProperty, value);
	}

	public string TotalText
	{
		get => (string)GetValue(TotalTextProperty);
		set => SetValue(TotalTextProperty, value);
	}

	public ICommand? IncreaseCommand
	{
		get => (ICommand?)GetValue(IncreaseCommandProperty);
		set => SetValue(IncreaseCommandProperty, value);
	}

	public ICommand? DecreaseCommand
	{
		get => (ICommand?)GetValue(DecreaseCommandProperty);
		set => SetValue(DecreaseCommandProperty, value);
	}

	public object? CommandParameter
	{
		get => GetValue(CommandParameterProperty);
		set => SetValue(CommandParameterProperty, value);
	}

	public ICommand? DeleteCommand
	{
		get => (ICommand?)GetValue(DeleteCommandProperty);
		set => SetValue(DeleteCommandProperty, value);
	}
}
