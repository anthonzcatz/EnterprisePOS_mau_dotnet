using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnterprisePOS.Models;

public sealed class PosNavItem : INotifyPropertyChanged
{
	private bool isActive;

	public event PropertyChangedEventHandler? PropertyChanged;

	public string Key { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;

	/// Unicode char or emoji used as icon glyph in the sidebar.
	public string Glyph { get; set; } = "•";

	public bool IsActive
	{
		get => isActive;
		set
		{
			if (isActive == value) return;
			isActive = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(ShowActiveBackground));
		}
	}

	public bool ShowActiveBackground => IsActive;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
