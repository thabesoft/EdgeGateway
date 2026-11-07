using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class PortViewModele(IPort port, ISukiToastManager sukiToastManager) : LifecycleViewModel(port, sukiToastManager), IAsyncDisposable
{
    private readonly AvaloniaDictionary<DeviceId, DeviceViewModel> _viewModels = new(
       port.Devices.Values
           .Select(x => KeyValuePair.Create(x.Id, new DeviceViewModel(x, sukiToastManager)))
           .ToDictionary()
   );


    public ConnectionEndpoint Endpoint => port.Endpoint;
    public IAvaloniaReadOnlyDictionary<DeviceId, DeviceViewModel> Devices => _viewModels;


    [RelayCommand]
    private void AddDevice(DeviceDescriptor deviceDescriptor)
    {
        var device = port.AddDevice(deviceDescriptor);
        _viewModels[deviceDescriptor.Id] = new DeviceViewModel(device, sukiToastManager);
    }

    [RelayCommand]
    private async Task RemoveDevice(DeviceId deviceId)
    {
        if (_viewModels.TryGetValue(deviceId, out var vm))
            await vm.DisposeAsync();

        _viewModels.Remove(deviceId);
    }

    public ValueTask DisposeAsync() => port.DisposeAsync();
}
