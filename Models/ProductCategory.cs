using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnterprisePOS.Models;

public sealed class ProductCategory : INotifyPropertyChanged
{
	private bool isSelected;

	public event PropertyChangedEventHandler? PropertyChanged;

	public string Key { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Icon { get; set; } = "📦";

	/// <summary>Remote or local image path — replace with your asset anytime.</summary>
	public string ImageUrl { get; set; } = string.Empty;

	public bool IsSelected
	{
		get => isSelected;
		set
		{
			if (isSelected == value)
			{
				return;
			}

			isSelected = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(IsSelectedUnderlineVisible));
		}
	}

	public bool IsSelectedUnderlineVisible => IsSelected;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
