using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnterprisePOS.Helpers;

public abstract class BaseViewModel : INotifyPropertyChanged
{
	private bool isBusy;

	public event PropertyChangedEventHandler? PropertyChanged;

	public bool IsBusy
	{
		get => isBusy;
		set => SetProperty(ref isBusy, value);
	}

	protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
		{
			return false;
		}

		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
