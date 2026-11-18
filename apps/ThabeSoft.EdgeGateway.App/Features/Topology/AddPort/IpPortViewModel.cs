using CommunityToolkit.Mvvm.ComponentModel;
using System.Net;
using ThabeSoft.EdgeGateway.Ethernet;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;


internal sealed partial class IpPortViewModel : ViewModelBase
{
    public IpProtocolType[] TransportTypeOptions { get; } = [IpProtocolType.Tcp, IpProtocolType.Udp];


    [ObservableProperty]
    public partial string Ip { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int? Port { get; set; } = 0;

    [ObservableProperty]
    public partial IpProtocolType TransportType { get; set; } = IpProtocolType.Tcp;


    partial void OnIpChanged(string value)
    {
        IpTransportDescriptor.ValidateIp(value, out var errorMessage);
        TrySetPropertyError(nameof(Ip), errorMessage);
    }
    partial void OnPortChanged(int? value)
    {
        IpTransportDescriptor.ValidatePort(value ?? 0, out var errorMessage);
        TrySetPropertyError(nameof(Port), errorMessage);
    }
    partial void OnTransportTypeChanged(IpProtocolType value)
    {
        IpTransportDescriptor.ValidateTransportType(value, out var errorMessage);
        TrySetPropertyError(nameof(TransportType), errorMessage);
    }

    public IpTransportDescriptor Build() =>
        new() { Port = Port.Value, IP = IPAddress.Parse(Ip), ProtocolType = TransportType };

    public void ValidateAllProperty()
    {
        OnIpChanged(Ip);
        OnPortChanged(Port);
        OnTransportTypeChanged(TransportType);
    }
}