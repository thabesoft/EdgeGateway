using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using ThabeSoft.EdgeGateway.App.Features.Gateway;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;


internal sealed partial class MenuItemViewModel : ViewModelBase
{
    private readonly AvaloniaList<MenuItemViewModel> _childs = [];


    [ObservableProperty]
    public partial string Header { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PortContent))]
    [NotifyPropertyChangedFor(nameof(IsPortContent))]
    [NotifyPropertyChangedFor(nameof(DeviceContent))]
    [NotifyPropertyChangedFor(nameof(IsDeviceContent))]
    public partial object Content { get; set; }

    public IAvaloniaReadOnlyList<MenuItemViewModel> ChildIs => _childs;


    public PortViewModel? PortContent => Content as PortViewModel;
    public DeviceViewModel? DeviceContent => Content as DeviceViewModel;

    public bool IsPortContent => PortContent is not null;
    public bool IsDeviceContent => DeviceContent is not null;



    public void AddChild(MenuItemViewModel item)
    {
        _childs.Add(item);
    }
    public void AddChilds(params IEnumerable<MenuItemViewModel> items)
    {
        _childs.AddRange(items);
    }
    public bool RemoveChild(MenuItemViewModel item)
    {
        return _childs.Remove(item);
    }
}