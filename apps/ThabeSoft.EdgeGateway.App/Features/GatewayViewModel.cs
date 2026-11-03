using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


internal sealed partial class GatewayViewModel(IGateway gateway, PortViewModelFactory portViewModelFactory) : ViewModelBase
{
    private readonly AvaloniaList<PortViewModel> _ports = [.. gateway.Ports.Values.Select(port =>
        portViewModelFactory(port))];


    [ObservableProperty]
    public partial bool IsRunning { get; private set; } = gateway.State == LifecycleState.Running;
    public IReadOnlyList<PortViewModel> Ports => _ports;






    [RelayCommand]
    private async Task StartAsync()
    {
        await gateway.StartAsync();
        IsRunning = gateway.State == LifecycleState.Running;
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        await gateway.StopAsync();
        IsRunning = gateway.State == LifecycleState.Stopped;
    }


    [RelayCommand]
    private void AddPort(IPortDescriptor portDescriptor)
    {
        if (portDescriptor is null) return;

        var port = gateway.AddPort(portDescriptor);
        var portVm = portViewModelFactory(port);

        _ports.Add(portVm);
    }

    [RelayCommand]
    private async Task RemovePortAsync(ChannelAddress channelAddress)
    {
        if (channelAddress is null) return;

        await gateway.RemovePortAsync(channelAddress);
        var portVm = _ports.FirstOrDefault(x => x.ChannelAddress == channelAddress);
        if (portVm is null) return;

        _ports.Remove(portVm);
    }
}


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


internal delegate DeviceViewModel DeviceViewModelFactory(IDevice device);
internal sealed partial class DeviceViewModel(IDevice device) : ViewModelBase
{
    public DeviceId Id => device.Id;
    public ProtocolType ProtocolType => device.ProtocolType;


    [ObservableProperty]
    public partial bool DisplayName { get; set; }

    [ObservableProperty]
    public partial bool IsEnabled { get; set; } = device.IsEnabled;


    [RelayCommand]
    private async Task EnableAsync()
    {
        await device.EnableAsync();
        IsEnabled = device.IsEnabled;
    }

    [RelayCommand]
    private async Task DisableAsync()
    {
        await device.DisableAsync();
        IsEnabled = device.IsEnabled;
    }
}