namespace EnterprisePOS.Components;

public partial class PageHeader : ContentView
{
	public static readonly BindableProperty TitleProperty =
		BindableProperty.Create(nameof(Title), typeof(string), typeof(PageHeader), string.Empty);

	public static readonly BindableProperty DescriptionProperty =
		BindableProperty.Create(nameof(Description), typeof(string), typeof(PageHeader), string.Empty);

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

	public bool HasDescription => !string.IsNullOrEmpty(Description);

	public PageHeader()
	{
		InitializeComponent();
	}
}
