using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnterprisePOS.Features.POS.Models;

public sealed class CartItem : INotifyPropertyChanged
{
	private int quantity = 1;

	public event PropertyChangedEventHandler? PropertyChanged;

	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Image { get; set; } = "dotnet_bot.png";
	public decimal Price { get; set; }
	public string Size { get; set; } = string.Empty;
	public string Color { get; set; } = string.Empty;

	public int Quantity
	{
		get => quantity;
		set
		{
			if (quantity == value)
			{
				return;
			}

			quantity = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(LineTotal));
		}
	}

	public decimal LineTotal => Price * Quantity;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
