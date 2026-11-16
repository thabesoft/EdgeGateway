using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class GatewayViewModel : LifecycleViewModel
{
    private readonly AvaloniaDictionary<PortId, PortViewModel> _viewModels;
    private readonly IGateway _gateway;
    private readonly IEventSubscriber _eventSubscriber;
    private readonly ISukiToastManager _sukiToastManager;


    public GatewayViewModel(
            IGateway gateway,
            IEventSubscriber eventSubscriber,
            ISukiToastManager sukiToastManager
        ) : base(gateway, sukiToastManager)
    {
        _gateway = gateway;
        _sukiToastManager = sukiToastManager;
        _eventSubscriber = eventSubscriber;
        _viewModels = new(gateway.Ports.ToDictionary(k => k.Key, v => CreateViewModel(v.Value)));

        IDisposable[] eventDisposers =
        [
            _eventSubscriber.Subscribe<PortAddedEvent>((e, _) =>
            {
                if (gateway.Ports.TryGetValue(e.PortId, out var port))
                    _viewModels[e.PortId] = CreateViewModel(port);

                return ValueTask.CompletedTask;
            }),
            _eventSubscriber.Subscribe<PortRemovedEvent>((e, _) =>
            {
                _viewModels.Remove(e.PortId);
                return ValueTask.CompletedTask;
            }),

             _eventSubscriber.Subscribe<DeviceAddedEvent>((e, _) =>
            {
                if (gateway.Ports.TryGetValue(e.PortId, out var port))
                    if(port.Devices.TryGetValue(e.DeviceId, out var device))
                        ;
                return ValueTask.CompletedTask;
            }),
            _eventSubscriber.Subscribe<DeviceRemovedEvent>((e, _) =>
            {
                _viewModels.Remove(e.PortId);
                return ValueTask.CompletedTask;
            }),
        ];
    }

    public IAvaloniaReadOnlyDictionary<PortId, PortViewModel> Ports => _viewModels;



    [RelayCommand]
    private async Task AddPortAsync(PortDescriptor portDescriptor)
    {
        await _sukiToastManager.ExecuteWithToastAsync(async ct =>
            await AddPortAsync(portDescriptor, ct), "添加端口", TimeSpan.FromSeconds(30));
    }

    [RelayCommand]
    private async Task RemovePort(PortId id)
    {
        await _sukiToastManager.ExecuteWithToastAsync(async ct =>
           await RemovePortAsync(id, ct), "删除端口", TimeSpan.FromSeconds(10));
    }

    public override async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        await base.StartAsync(cancellationToken);

        foreach (var i in _viewModels.Values)
            i.OnUpdateState();
    }
    public override async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        await base.StopAsync(cancellationToken);

        foreach (var i in _viewModels.Values)
            i.OnUpdateState();
    }

    public PortViewModel AddPort(IPort port)
    {
        var vm = CreateViewModel(port);
        _viewModels[port.Id] = vm;

        return vm;
    }


    public async ValueTask<PortViewModel> AddPortAsync(PortDescriptor portDescriptor, CancellationToken cancellationToken = default)
    {
        var model = await _gateway.AddPortAsync(portDescriptor);
        var viewModel = CreateViewModel(model);

        _viewModels[portDescriptor.Id] = viewModel;
        return viewModel;
    }
    public async ValueTask<bool> RemovePortAsync(PortId id, CancellationToken cancellationToken = default)
    {
        var result = await _gateway.RemovePortAsync(id, cancellationToken);
        _viewModels.Remove(id);

        return result;
    }

    private PortViewModel CreateViewModel(IPort port) => new(port, _sukiToastManager);
}