using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class GatewayViewModel : LifecycleViewModel
{
    private readonly AvaloniaDictionary<PortId, PortViewModel> _viewModels;
    private readonly IGateway _gateway;
    private readonly ISukiToastManager _sukiToastManager;


    public GatewayViewModel(IGateway gateway, ISukiToastManager sukiToastManager) : base(gateway, sukiToastManager)
    {
        _gateway = gateway;
        _sukiToastManager = sukiToastManager;
        _viewModels = new(gateway.Ports.ToDictionary(k => k.Key, v => CreateViewModel(v.Value)));
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