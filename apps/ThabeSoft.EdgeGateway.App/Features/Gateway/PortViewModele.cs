using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Transports;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class PortViewModel : ObservableObject, IDisposable
{
    private readonly AvaloniaDictionary<DeviceId, DeviceViewModel> _viewModels;
    private readonly IPort _port;
    private readonly ISukiToastManager _sukiToastManager;
    private readonly IDisposable[] _eventDisposers;


    public PortViewModel(IPort port, IEventSubscriber eventSubscriber, ISukiToastManager sukiToastManager)
    {
        _port = port;
        _sukiToastManager = sukiToastManager;
        _viewModels = new(port.Devices.ToDictionary(k => k.Key, v => CreateViewModel(v.Value)));


        _eventDisposers =
        [
             eventSubscriber.Subscribe<DeviceAddedEvent>((e, _) =>
            {
                if(e.PortId != port.Id)return ValueTask.CompletedTask;

                if(port.Devices.TryGetValue(e.DeviceId, out var device))
                        _viewModels[e.DeviceId] = CreateViewModel(device);

                return ValueTask.CompletedTask;
            }),
            eventSubscriber.Subscribe<DeviceRemovedEvent>((e, _) =>
            {
                if(e.PortId != port.Id) return ValueTask.CompletedTask;

                _viewModels.Remove(e.DeviceId);
                return ValueTask.CompletedTask;
            }),
            eventSubscriber.Subscribe<PortStateChangedEvent>((e, _) =>
            {
                if(e.PortId != port.Id) return ValueTask.CompletedTask;

                State = e.New;
                return ValueTask.CompletedTask;
            })
        ];

        DeviceViewModel CreateViewModel(IDevice device) => new(device, eventSubscriber, _sukiToastManager);
    }


    public PortId Id => _port.Id;
    public ConnectionEndpoint Endpoint => _port.Endpoint;
    public IAvaloniaReadOnlyDictionary<DeviceId, DeviceViewModel> Devices => _viewModels;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRunning))]
    public partial LifecycleState State { get; private set; }
    public bool IsRunning => State == LifecycleState.Running;



    [RelayCommand]
    private async Task StartAsync()
    {
        await _sukiToastManager.ExecuteWithToastAsync(_port.StartAsync, "启动端口");
    }
    [RelayCommand]
    private async Task StopAsync()
    {
        await _sukiToastManager.ExecuteWithToastAsync(_port.StopAsync, "停止端口");
    }
    [RelayCommand]
    private async Task RemoveDeviceAsync(DeviceId deviceId)
    {
        await _sukiToastManager.ExecuteWithToastAsync(async ct =>
            await _port.RemoveDeviceAsync(deviceId, ct), "删除设备");
    }



    public void Dispose()
    {
        foreach (var i in _eventDisposers)
            i.Dispose();
    }
}
