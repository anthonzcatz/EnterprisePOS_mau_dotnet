namespace EnterprisePOS.Components;

public partial class PosCategoryTabView : ContentView
{
	public static readonly BindableProperty TitleProperty =
		BindableProperty.Create(nameof(Title), typeof(string), typeof(PosCategoryTabView), string.Empty);

	public static readonly BindableProperty IconProperty =
		BindableProperty.Create(nameof(Icon), typeof(string), typeof(PosCategoryTabView), "📦");

	public static readonly BindableProperty ImageUrlProperty =
		BindableProperty.Create(nameof(ImageUrl), typeof(string), typeof(PosCategoryTabView), string.Empty);

	public static readonly BindableProperty IsSelectedProperty =
		BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(PosCategoryTabView), false);

	public PosCategoryTabView()
	{
		InitializeComponent();
	}

	public string Title
	{
		get => (string)GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public string Icon
	{
		get => (string)GetValue(IconProperty);
		set => SetValue(IconProperty, value);
	}

	public string ImageUrl
	{
		get => (string)GetValue(ImageUrlProperty);
		set => SetValue(ImageUrlProperty, value);
	}

	public bool IsSelected
	{
		get => (bool)GetValue(IsSelectedProperty);
		set => SetValue(IsSelectedProperty, value);
	}
}
