using CommunityToolkit.Mvvm.ComponentModel;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.Polling;

namespace ThabeSoft.EdgeGateway.App.ViewModels;

internal sealed partial class TagViewModel(ITagMonitor monitor) : ViewModelBase
{
    [ObservableProperty]
    public partial string Name { get; set; } = monitor.Name.ToString();

    [ObservableProperty]
    public partial string Address { get; set; } = monitor.Address.ToString() ?? "";


    [ObservableProperty]
    public partial TagValueState State { get; set; } = monitor.State;

    [ObservableProperty]
    public partial object? Value { get; set; } = monitor.Value;

    [ObservableProperty]
    public partial DateTimeOffset UpdateTime { get; set; } = monitor.UpdateTime;
}