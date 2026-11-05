using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


internal delegate DeviceViewModel DeviceViewModelFactory(IDevice device);

internal sealed partial class DeviceViewModel(IDevice device) : ViewModelBase
{
    private readonly AvaloniaList<TagViewModel> _tagMonitors = [..device.TagMonitors.Values.Select(device =>
        new TagViewModel(device))];


    public DeviceId Id => device.Id;
    public ProtocolType ProtocolType => device.ProtocolType;
    public ICollection<TagViewModel> TagMonitors => _tagMonitors;



    [ObservableProperty]
    public partial bool DisplayName { get; set; }

    [ObservableProperty]
    public partial bool IsEnabled { get; set; } = device.IsRunning;


    [RelayCommand]
    private async Task EnableAsync()
    {
        await device.StartAsync();
        RefreshIsEnabled();
    }

    [RelayCommand]
    private async Task DisableAsync()
    {
        await device.StopAsync();
        RefreshIsEnabled();
    }


    public void RefreshIsEnabled() => IsEnabled = device.IsRunning;
}