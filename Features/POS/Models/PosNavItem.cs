using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnterprisePOS.Features.POS.Models;

public class PosNavItem : INotifyPropertyChanged
{
    private bool isActive;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Glyph { get; set; } = string.Empty;

    public bool IsActive
    {
        get => isActive;
        set
        {
            if (isActive == value)
            {
                return;
            }

            isActive = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
