using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI.Primitives.Extensions;
using SukiUI.Toasts;
using System.Net;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class GatewayViewModel : ObservableObject, IDisposable
{
    private readonly AvaloniaDictionary<PortId, PortViewModel> _viewModels;
    private readonly IGateway _gateway;
    private readonly ISukiToastManager _sukiToastManager;
    private readonly IDisposable[] _eventDisposers;


    public GatewayViewModel(
            IGateway gateway,
            IEventSubscriber eventSubscriber,
            ISukiToastManager sukiToastManager
        )
    {
        _gateway = gateway;
        _sukiToastManager = sukiToastManager;
        _viewModels = new(gateway.Ports.ToDictionary(k => k.Key, v => CreateViewModel(v.Value)));

        _eventDisposers =
        [
            eventSubscriber.Subscribe<PortAddedEvent>((e, _) =>
            {
                if (gateway.Ports.TryGetValue(e.PortId, out var port))
                {
                    _viewModels[e.PortId] = CreateViewModel(port);
                }

                return ValueTask.CompletedTask;
            }),
            eventSubscriber.Subscribe<PortRemovedEvent>((e, _) =>
            {
                 Dispatcher.UIThread.InvokeAsync(() =>
                    _viewModels.Remove(e.PortId));
                return ValueTask.CompletedTask;
            }),
            eventSubscriber.Subscribe<GatewayStateChangedEvent>((e, _) =>
            {
                Dispatcher.UIThread.InvokeAsync(() => State = e.New);
                return ValueTask.CompletedTask;
            })
        ];

        // 创建视图模型
        PortViewModel CreateViewModel(IPort port) => new(port, eventSubscriber, sukiToastManager);
    }

    public IAvaloniaReadOnlyDictionary<PortId, PortViewModel> Ports => _viewModels;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRunning))]
    public partial LifecycleState State { get; private set; }
    public bool IsRunning => State == LifecycleState.Running;


    [RelayCommand]
    private async Task StartAsync()
    {
        await _sukiToastManager.ExecuteWithToastAsync(_gateway.StartAsync, "启动网关");
    }
    [RelayCommand]
    private async Task StopAsync()
    {
        await _sukiToastManager.ExecuteWithToastAsync(_gateway.StopAsync, "启动网关");
    }
    [RelayCommand]
    private async Task RemovePortAsync(PortId id)
    {
        await _sukiToastManager.ExecuteWithToastAsync(async ct =>
            await _gateway.RemovePortAsync(id, ct), "删除端口");
    }


    public void Dispose()
    {
        foreach (var i in _eventDisposers)
            i.Dispose();
    }
}