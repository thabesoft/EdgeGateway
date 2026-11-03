using CommunityToolkit.Mvvm.ComponentModel;
using System.Net;
using ThabeSoft.EdgeGateway.Channels;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


internal sealed partial class IpPortFormViewModel : ViewModelBase
{
    public IpTransportType[] ConnectionTypeOptions { get; } = [IpTransportType.Tcp, IpTransportType.Udp];


    [ObservableProperty]
    public partial string Ip { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int? Port { get; set; } = 0;

    [ObservableProperty]
    public partial IpTransportType ConnectionType { get; set; } = IpTransportType.Tcp;


    partial void OnIpChanged(string value)
    {
        IpChannelDescriptor.ValidateIp(value, out var errorMessage);
        TrySetPropertyError(nameof(Ip), errorMessage);
    }
    partial void OnPortChanged(int? value)
    {
        IpChannelDescriptor.ValidatePort(value ?? 0, out var errorMessage);
        TrySetPropertyError(nameof(Port), errorMessage);
    }
    partial void OnConnectionTypeChanged(IpTransportType value)
    {
        IpChannelDescriptor.ValidateTransportType(value, out var errorMessage);
        TrySetPropertyError(nameof(ConnectionType), errorMessage);
    }



    public IpChannelDescriptor Build()
    {
        return IpChannelDescriptor.Create(ConnectionType, new(IPAddress.Parse(Ip), Port ?? 0));
    }
}