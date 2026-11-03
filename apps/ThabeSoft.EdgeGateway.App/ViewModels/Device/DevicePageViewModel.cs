using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ThabeSoft.EdgeGateway.App.Messages;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


public partial class DevicePageViewModel : ViewModelBase, IRecipient<DeviceRemovedMessage>
{
    private readonly IPort _port;

    public DevicePageViewModel(IPort port)
    {
        _port = port;
        WeakReferenceMessenger.Default.Register(this);
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
    void IRecipient<DeviceRemovedMessage>.Receive(DeviceRemovedMessage message)
    {
        var portVm = Devices.FirstOrDefault(x => x.Id == message.DeviceId);
        if (portVm is null) return;

        Devices.Remove(portVm);
    }
}

public sealed partial class DeviceItemViewModel(IDevice device, IPort port) : ViewModelBase
{
    public DeviceId Id => device.Id;
    public ProtocolType ProtocolType => device.ProtocolType;
    public bool IsEnabled => device.IsEnabled;
    public int MonitorsCount => device.Monitors.Count;


    [RelayCommand]
    private async Task EnableAsync()
    {
        await device.EnableAsync();
        OnPropertyChanged(nameof(IsEnabled));
    }

    [RelayCommand]
    private async Task DisposeAsync()
    {
        await device.DisposeAsync();
        OnPropertyChanged(nameof(IsEnabled));
    }

    [RelayCommand]
    private async Task RemoveAsync()
    {
        await port.RemoveDeviceAsync(Id);
        WeakReferenceMessenger.Default.Send(new DeviceRemovedMessage(port.ChannelAddress, Id));
    }

    [RelayCommand]
    private async Task OpenMonitorAsync()
    {
        await port.RemoveDeviceAsync(Id);
        WeakReferenceMessenger.Default.Send(new DeviceRemovedMessage(port.ChannelAddress, Id));
    }
}