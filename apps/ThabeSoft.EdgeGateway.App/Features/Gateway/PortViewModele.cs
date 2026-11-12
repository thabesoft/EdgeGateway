using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Services;
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
        _viewModels = new(port.Devices.ToDictionary(k => k.Key, v => CreateViewModel(v.Value)));
    }

    public ConnectionEndpoint Endpoint => _port.Endpoint;
    public IAvaloniaReadOnlyDictionary<DeviceId, DeviceViewModel> Devices => _viewModels;



    [RelayCommand]
    private async Task AddDeviceAsync(DeviceDescriptor deviceDescriptor)
    {
        await _sukiToastManager.ExecuteWithToastAsync(async ct =>
           await AddDeviceAsync(deviceDescriptor, ct), "添加设备", TimeSpan.FromSeconds(30));
    }

    [RelayCommand]
    private async Task RemoveDeviceAsync(DeviceId deviceId)
    {
        await _sukiToastManager.ExecuteWithToastAsync(async ct =>
           await RemoveDeviceAsync(deviceId, ct), "删除设备", TimeSpan.FromSeconds(10));
    }


    public async ValueTask<DeviceViewModel> AddDeviceAsync(DeviceDescriptor deviceDescriptor, CancellationToken cancellationToken = default)
    {
        var model = await _port.AddDeviceAsync(deviceDescriptor);
        var viewModel = CreateViewModel(model);

        _viewModels[deviceDescriptor.Id] = viewModel;
        return viewModel;
    }
    public async ValueTask<bool> RemoveDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default)
    {
        var result = await _port.RemoveDeviceAsync(deviceId);
        _viewModels.Remove(deviceId);

        return result;
    }

    private DeviceViewModel CreateViewModel(IDevice device) => new(device, _sukiToastManager);
}
