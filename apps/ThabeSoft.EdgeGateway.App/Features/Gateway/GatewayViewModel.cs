using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


internal sealed partial class GatewayViewModel(IGateway gateway, PortViewModelFactory portViewModelFactory) : ViewModelBase
{
    private readonly AvaloniaList<PortViewModel> _ports = [.. gateway.Ports.Values.Select(port =>
        portViewModelFactory(port))];


    [ObservableProperty]
    public partial bool IsRunning { get; private set; } = gateway.State == LifecycleState.Running;
    public IReadOnlyList<PortViewModel> Ports => _ports;






    [RelayCommand]
    private async Task StartAsync()
    {
        await gateway.StartAsync();
        IsRunning = gateway.State == LifecycleState.Running;
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        await gateway.StopAsync();
        IsRunning = gateway.State == LifecycleState.Stopped;
    }


    [RelayCommand]
    private void AddPort(IPortDescriptor portDescriptor)
    {
        if (portDescriptor is null) return;

        var port = gateway.AddPort(portDescriptor);
        var portVm = portViewModelFactory(port);

        _ports.Add(portVm);
    }

    [RelayCommand]
    private async Task RemovePortAsync(ChannelAddress channelAddress)
    {
        if (channelAddress is null) return;

        await gateway.RemovePortAsync(channelAddress);
        var portVm = _ports.FirstOrDefault(x => x.ChannelAddress == channelAddress);
        if (portVm is null) return;

        _ports.Remove(portVm);
    }
}