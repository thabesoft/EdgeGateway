using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Net;
using ThabeSoft.EdgeGateway.Channels;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;


internal sealed partial class IpPortFormViewModel : ViewModelBase
{
    public IpTransportType[] TransportTypeOptions { get; } = [IpTransportType.Tcp, IpTransportType.Udp];


    [ObservableProperty]
    public partial string Ip { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int? Port { get; set; } = 0;

    [ObservableProperty]
    public partial IpTransportType TransportType { get; set; } = IpTransportType.Tcp;


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
    partial void OnTransportTypeChanged(IpTransportType value)
    {
        IpChannelDescriptor.ValidateTransportType(value, out var errorMessage);
        TrySetPropertyError(nameof(TransportType), errorMessage);
    }

    public IpChannelDescriptor Build() =>
        IpChannelDescriptor.Create(TransportType, new IPEndPoint(IPAddress.Parse(Ip), Port!.Value));

    public void ValidateAllProperty()
    {
        OnIpChanged(Ip);
        OnPortChanged(Port);
        OnTransportTypeChanged(TransportType);
    }
}