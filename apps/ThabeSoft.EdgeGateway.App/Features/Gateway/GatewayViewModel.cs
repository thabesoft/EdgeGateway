using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class GatewayViewModel(IGateway gateway, ISukiToastManager sukiToastManager) : LifecycleViewModel(gateway, sukiToastManager)
{
    private readonly AvaloniaDictionary<ConnectionEndpoint, PortViewModele> _viewModels = new(
        gateway.Ports.Values
            .Select(x => KeyValuePair.Create(x.Endpoint, new PortViewModele(x, sukiToastManager)))
            .ToDictionary()
    );


    public IAvaloniaReadOnlyDictionary<ConnectionEndpoint, PortViewModele> Ports => _viewModels;



    [RelayCommand]
    private void AddPort(ITransportDescriptor transportDescriptor)
    {
        var port = gateway.AddPortAsync(transportDescriptor);
        _viewModels[transportDescriptor.Endpoint] = new PortViewModele(port, sukiToastManager);
    }

    [RelayCommand]
    private async Task RemovePort(ConnectionEndpoint connectionEndpoint)
    {
        if (_viewModels.TryGetValue(connectionEndpoint, out var vm))
            await vm.DisposeAsync();

        _viewModels.Remove(connectionEndpoint);
    }
}
