using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;

namespace ThabeSoft.EdgeGateway.App.Features;


internal abstract partial class PageViewModel(string displayName, MaterialIconKind icon, int index = 0) : ObservableValidator
{
    [ObservableProperty]
    public partial string DisplayName { get; set; } = displayName;

    [ObservableProperty]
    public partial MaterialIconKind Icon { get; set; } = icon;

    [ObservableProperty]
    public partial int Index { get; set; } = index;
}