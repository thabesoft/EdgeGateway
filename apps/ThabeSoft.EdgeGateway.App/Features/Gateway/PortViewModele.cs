using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class PortViewModel : LifecycleViewModel
{
    private readonly AvaloniaDictionary<DeviceId, DeviceViewModel> _viewModels;
    private readonly IPort _port;
    private readonly ISukiToastManager _sukiToastManager;


    public PortViewModel(IPort port, ISukiToastManager sukiToastManager) : base(port, sukiToastManager)
    {
        _port = port;
        _sukiToastManager = sukiToastManager;

        _viewModels = new(port.Devices.Values
           .Select(x => KeyValuePair.Create(x.Id, CreatePortViewModel(x)))
           .ToDictionary());
    }

    public ConnectionEndpoint Endpoint => _port.Endpoint;
    public IAvaloniaReadOnlyDictionary<DeviceId, DeviceViewModel> Devices => _viewModels;



    [RelayCommand]
    private async Task AddDeviceAsync(DeviceDescriptor deviceDescriptor)
    {
        var device = await _port.AddDeviceAsync(deviceDescriptor);
        _viewModels.TryAdd(deviceDescriptor.Id, CreatePortViewModel(device));
    }

    [RelayCommand]
    private async Task RemoveDeviceAsync(DeviceId deviceId)
    {
        await _port.RemoveDeviceAsync(deviceId);
        _viewModels.Remove(deviceId);
    }



    private DeviceViewModel CreatePortViewModel(IDevice device) =>
        new(device, _sukiToastManager);
}
