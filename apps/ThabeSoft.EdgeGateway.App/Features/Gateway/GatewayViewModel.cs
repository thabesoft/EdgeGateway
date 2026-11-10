using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
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

        _viewModels = new(gateway.Ports.Values
            .Select(port => KeyValuePair.Create(port.Endpoint, CreatePortViewModel(port)))
            .ToDictionary());
    }

    public IAvaloniaReadOnlyDictionary<ConnectionEndpoint, PortViewModel> Ports => _viewModels;



    [RelayCommand]
    private async Task AddPort(ITransportDescriptor transportDescriptor)
    {
        var port = await _gateway.AddPortAsync(transportDescriptor);
        _viewModels.TryAdd(transportDescriptor.Endpoint, CreatePortViewModel(port));
    }

    [RelayCommand]
    private async Task RemovePort(ConnectionEndpoint connectionEndpoint)
    {
        await _gateway.RemovePortAsync(connectionEndpoint);
        _viewModels.Remove(connectionEndpoint);
    }


    private PortViewModel CreatePortViewModel(IPort port) =>
         new(port, _sukiToastManager);
}