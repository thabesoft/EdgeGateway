using System.Net;

namespace EdgeGateway.Ports.Ethernet;

public sealed record class EthernetPortConfig : IPortConfig
{
    public EthernetType Type { get; private set; } = EthernetType.Tcp;
    public PortAddress Address { get; private set; } = null!;
    public IPEndPoint Remote { get; private set; } = null!;
    public IPEndPoint? Local { get; private set; }


    private EthernetPortConfig() { }

    public static EthernetPortConfig Tcp(IPEndPoint remote)
    {
        return new EthernetPortConfig()
        {
            Type = EthernetType.Tcp,
            Remote = remote,
            Address = PortAddress.Create(PortType.Ethernet, remote.ToString())
        };
    }
    public static EthernetPortConfig Udp(IPEndPoint remote)
    {
        return new EthernetPortConfig()
        {
            Type = EthernetType.Udp,
            Remote = remote,
            Address = PortAddress.Create(PortType.Ethernet, remote.ToString())
        };
    }


    public EthernetPortConfig SetLocalAddress(IPEndPoint iPEndPoint)
    {
        Local = iPEndPoint;
        return this;
    }
}


public enum EthernetType
{
    Tcp,

    Udp
}