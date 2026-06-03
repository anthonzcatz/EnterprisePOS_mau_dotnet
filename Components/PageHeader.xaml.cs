namespace EnterprisePOS.Components;

public partial class PageHeader : ContentView
{
	public static readonly BindableProperty EyebrowProperty =
		BindableProperty.Create(
			nameof(Eyebrow),
			typeof(string),
			typeof(PageHeader),
			string.Empty,
			propertyChanged: OnHeaderPropertyChanged);

	public static readonly BindableProperty TitleProperty =
		BindableProperty.Create(
			nameof(Title),
			typeof(string),
			typeof(PageHeader),
			string.Empty,
			propertyChanged: OnHeaderPropertyChanged);

	public static readonly BindableProperty DescriptionProperty =
		BindableProperty.Create(
			nameof(Description),
			typeof(string),
			typeof(PageHeader),
			string.Empty,
			propertyChanged: OnHeaderPropertyChanged);

	public static readonly BindableProperty MetaTextProperty =
		BindableProperty.Create(
			nameof(MetaText),
			typeof(string),
			typeof(PageHeader),
			string.Empty,
			propertyChanged: OnHeaderPropertyChanged);

	public static readonly BindableProperty TitleFontSizeProperty =
		BindableProperty.Create(nameof(TitleFontSize), typeof(double), typeof(PageHeader), 30d);

	public static readonly BindableProperty CenterOnMobileProperty =
		BindableProperty.Create(nameof(CenterOnMobile), typeof(bool), typeof(PageHeader), false);

	public string Eyebrow
	{
		get => (string)GetValue(EyebrowProperty);
		set => SetValue(EyebrowProperty, value);
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

	public string MetaText
	{
		get => (string)GetValue(MetaTextProperty);
		set => SetValue(MetaTextProperty, value);
	}

	public double TitleFontSize
	{
		get => (double)GetValue(TitleFontSizeProperty);
		set => SetValue(TitleFontSizeProperty, value);
	}

	public bool CenterOnMobile
	{
		get => (bool)GetValue(CenterOnMobileProperty);
		set => SetValue(CenterOnMobileProperty, value);
	}

	public bool HasEyebrow => !string.IsNullOrWhiteSpace(Eyebrow);
	public bool HasDescription => !string.IsNullOrEmpty(Description);
	public bool HasMetaText => !string.IsNullOrWhiteSpace(MetaText);

	public PageHeader()
	{
		InitializeComponent();
	}

	private static void OnHeaderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is not PageHeader header)
		{
			return;
		}

		header.OnPropertyChanged(nameof(HasEyebrow));
		header.OnPropertyChanged(nameof(HasDescription));
		header.OnPropertyChanged(nameof(HasMetaText));
	}
}
