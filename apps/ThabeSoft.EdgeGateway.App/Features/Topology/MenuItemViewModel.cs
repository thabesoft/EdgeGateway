using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;


public sealed partial class MenuItemViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Header { get; set; }

    [ObservableProperty]
    public partial object Content { get; set; }

    public IAvaloniaReadOnlyList<MenuItemViewModel> ChildItemsSource { get; init; } = new AvaloniaList<MenuItemViewModel>();
}