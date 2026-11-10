//using Avalonia.Collections;
//using CommunityToolkit.Mvvm.Input;
//using ThabeSoft.EdgeGateway.App.Features.Gateway;
//using ThabeSoft.EdgeGateway.Devices;
//using ThabeSoft.EdgeGateway.Lifecycles;
//using ThabeSoft.EdgeGateway.Ports;
//using ThabeSoft.EdgeGateway.Protocols;

//namespace ThabeSoft.EdgeGateway.App.Features.Topology;


//internal partial class DevicePageViewModel(IPort port) : ViewModelBase
//{
//    public AvaloniaList<DeviceItemViewModel> Devices { get; } = [];


//    [RelayCommand]
//    private async Task AddModbusAsciiDeviceAsync()
//    {
//        var device = await port.AddDeviceAsync(DeviceDescriptor.Create(Modbus.Ascii));
//        AddDevice(device);
//    }
//    [RelayCommand]
//    private async Task AddModbusRtuDeviceAsync()
//    {
//        var device = await port.AddDeviceAsync(DeviceDescriptor.Create(Modbus.Rtu));
//        AddDevice(device);
//    }
//    [RelayCommand]
//    private async Task AddModbusIpDeviceAsync()
//    {
//        var device = await port.AddDeviceAsync(DeviceDescriptor.Create(Modbus.Ip));
//        AddDevice(device);
//    }



//    private void AddDevice(IDevice device)
//    {
//        Devices.Add(new DeviceItemViewModel(device, port));
//    }
//}

//internal sealed partial class DeviceItemViewModel(IDevice device, PortViewModel portViewModele) : ViewModelBase
//{
//    public DeviceId Id => device.Id;
//    public ProtocolType ProtocolType => device.ProtocolType;
//    public bool IsEnabled => device.IsRunning;
//    public int SubscriptionsCount => device.Subscriptions.Count;


//    [RelayCommand]
//    private async Task EnableAsync()
//    {
//        await device.StartAsync();
//        OnPropertyChanged(nameof(IsEnabled));
//    }

//    [RelayCommand]
//    private async Task DisposeAsync()
//    {
//        await device.StopAsync();
//        OnPropertyChanged(nameof(IsEnabled));
//    }

//    [RelayCommand]
//    private async Task RemoveAsync()
//    {
//        await portViewModele.RemoveDeviceCommand.ExecuteAsync(Id);
//    }
//}