using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace EnterprisePOS.Features.POS.Models;

public class PosNavItem : INotifyPropertyChanged
{
    private bool isActive;
    private bool isExpanded;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Glyph { get; set; } = string.Empty;
    public string? Route { get; set; }
    public bool IsChild { get; set; }
    public ObservableCollection<PosNavItem> Children { get; } = [];
    public bool HasChildren => Children.Count > 0;

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

    public bool IsExpanded
    {
        get => isExpanded;
        set
        {
            if (isExpanded == value)
            {
                return;
            }

            isExpanded = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
