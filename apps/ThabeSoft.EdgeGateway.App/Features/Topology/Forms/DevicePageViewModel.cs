using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;


public partial class DevicePageViewModel : ViewModelBase
{
    private readonly IPort _port;

    public DevicePageViewModel(IPort port)
    {
        _port = port;
    }

    public AvaloniaList<DeviceItemViewModel> Devices { get; } = [];


    [RelayCommand]
    private void AddModbusAsciiDevice()
    {
        var device = _port.AddDevice(ModbusProtocolDescriptor.Ascii);
        AddDevice(device);
    }
    [RelayCommand]
    private void AddModbusRtuDevice()
    {
        var device = _port.AddDevice(ModbusProtocolDescriptor.Rtu);
        AddDevice(device);
    }
    [RelayCommand]
    private void AddModbusIpDevice()
    {
        var device = _port.AddDevice(ModbusProtocolDescriptor.Ip);
        AddDevice(device);
    }



    private void AddDevice(IDevice device)
    {
        Devices.Add(new DeviceItemViewModel(device, _port));
    }
}

public sealed partial class DeviceItemViewModel(IDevice device, IPort port) : ViewModelBase
{
    public DeviceId Id => device.Id;
    public ProtocolType ProtocolType => device.ProtocolType;
    public bool IsEnabled => device.IsRunning;
    public int MonitorsCount => device.TagMonitors.Count;


    [RelayCommand]
    private async Task EnableAsync()
    {
        await device.StartAsync();
        OnPropertyChanged(nameof(IsEnabled));
    }

    [RelayCommand]
    private async Task DisposeAsync()
    {
        await device.StopAsync();
        OnPropertyChanged(nameof(IsEnabled));
    }

    [RelayCommand]
    private async Task RemoveAsync()
    {
        await port.RemoveDeviceAsync(Id);
    }

    [RelayCommand]
    private async Task OpenMonitorAsync()
    {
        await port.RemoveDeviceAsync(Id);
    }
}