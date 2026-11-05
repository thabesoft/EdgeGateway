using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.App.ViewModels;

internal delegate PortViewModel PortViewModelFactory(IPort port);

internal sealed partial class PortViewModel(IPort port, DeviceViewModelFactory deviceViewModelFactory) : ViewModelBase
{
    private readonly AvaloniaList<DeviceViewModel> _devices = [.. port.Devices.Values.Select(device =>
        deviceViewModelFactory(device))];


    public ICollection<DeviceViewModel> Devices => _devices;
    public ConnectionEndpoint ChannelAddress => port.Endpoint;



    [ObservableProperty]
    public partial bool IsOpen { get; set; } = port.IsRunning;


    [RelayCommand]
    private async Task OpenAsync()
    {
        await port.StartAsync();
        RefreshIsOpen();
    }

    [RelayCommand]
    private async Task CloseAsync()
    {
        await port.StopAsync();
        RefreshIsOpen();
    }

    [RelayCommand]
    private void AddDevice(DeviceDescriptor deviceDescriptor)
    {
        var device = port.AddDeviceAsync(deviceDescriptor);
        var deviceVm = deviceViewModelFactory(device);

        _devices.Add(deviceVm);
    }

    [RelayCommand]
    private void AddDeviceFromProtocol(IProtocolDescriptor protocolDescriptor)
    {
        var device = port.AddDeviceAsync(protocolDescriptor);
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

    public void RefreshIsOpen()
    {
        IsOpen = port.IsRunning;
        foreach (var device in Devices) device.RefreshIsEnabled();
    }
}
