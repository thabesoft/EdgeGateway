using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class GatewayViewModel : LifecycleViewModel
{
    private readonly AvaloniaDictionary<ConnectionEndpoint, PortViewModel> _viewModels;
    private readonly IGateway _gateway;
    private readonly ISukiToastManager _sukiToastManager;


    public GatewayViewModel(IGateway gateway, ISukiToastManager sukiToastManager) : base(gateway, sukiToastManager)
    {
        _gateway = gateway;
        _sukiToastManager = sukiToastManager;
        _viewModels = new(gateway.Ports.ToDictionary(k => k.Key, v => CreateViewModel(v.Value)));
    }

    public IAvaloniaReadOnlyDictionary<ConnectionEndpoint, PortViewModel> Ports => _viewModels;



    [RelayCommand]
    private async Task AddPortAsync(PortDescriptor portDescriptor)
    {
        await _sukiToastManager.ExecuteWithToastAsync(async ct =>
            await AddPortAsync(portDescriptor, ct), "添加端口", TimeSpan.FromSeconds(30));
    }

    [RelayCommand]
    private async Task RemovePort(ConnectionEndpoint connectionEndpoint)
    {
        await _sukiToastManager.ExecuteWithToastAsync(async ct =>
           await RemovePortAsync(connectionEndpoint, ct), "删除端口", TimeSpan.FromSeconds(10));
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

        _viewModels[portDescriptor.Transport.Endpoint] = viewModel;
        return viewModel;
    }
    public async ValueTask<bool> RemovePortAsync(ConnectionEndpoint connectionEndpoint, CancellationToken cancellationToken = default)
    {
        var result = await _gateway.RemovePortAsync(connectionEndpoint);
        _viewModels.Remove(connectionEndpoint);

        return result;
    }

    private PortViewModel CreateViewModel(IPort port) => new(port, _sukiToastManager);
}