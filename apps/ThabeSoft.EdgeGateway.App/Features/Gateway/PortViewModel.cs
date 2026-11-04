using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.App.ViewModels;

internal delegate PortViewModel PortViewModelFactory(IPort port);

internal sealed partial class PortViewModel(IPort port, DeviceViewModelFactory deviceViewModelFactory) : ViewModelBase
{
    private readonly AvaloniaList<DeviceViewModel> _devices = [.. port.Devices.Values.Select(device =>
        deviceViewModelFactory(device))];


    public ICollection<DeviceViewModel> Devices => _devices;
    public ChannelAddress ChannelAddress => port.ChannelAddress;



    [ObservableProperty]
    public partial bool IsOpen { get; set; } = port.IsOpen;


    [RelayCommand]
    private async Task OpenAsync()
    {
        await port.OpenAsync();
        IsOpen = port.IsOpen;
    }

    [RelayCommand]
    private async Task CloseAsync()
    {
        await port.CloseAsync();
        IsOpen = port.IsOpen;
    }

    [RelayCommand]
    private void AddDevice(IDeviceDescriptor deviceDescriptor)
    {
        var device = port.AddDevice(deviceDescriptor);
        var deviceVm = deviceViewModelFactory(device);

        _devices.Add(deviceVm);
    }

    [RelayCommand]
    private void AddDeviceFromProtocol(IProtocolDescriptor protocolDescriptor)
    {
        var device = port.AddDevice(protocolDescriptor);
        var deviceVm = deviceViewModelFactory(device);

        _devices.Add(deviceVm);
    }


    [RelayCommand]
    private async Task RemoveDeviceAsync(DeviceId deviceId)
    {
        await port.RemoveDeviceAsync(deviceId);
        var deviceVm = _devices.FirstOrDefault(x => x.Id == deviceId);
        if (deviceVm is null) return;

        _devices.Remove(deviceVm);
    }
}
