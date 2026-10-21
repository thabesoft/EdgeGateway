using EdgeGateway.Ports;
using System.Net;

namespace EdgeGateway.Ethernet;


/// <summary>
/// 以太网端口配置
/// </summary>
public sealed record class EthernetPortOptions : IPortOptions
{
    public required PortAddress Address { get; init; }
    public required IPEndPoint Remote { get; init; }
    public IPEndPoint? Local { get; init; }


    internal EthernetPortOptions() { }


    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public static EthernetPortOptionsBuilder Builder(TransportProtocol portType, IPEndPoint remote)
    {
        if (portType == TransportProtocol.Tcp || portType == TransportProtocol.Udp)
        {
            return new EthernetPortOptionsBuilder().SetType(portType).SetRemote(remote);
        }

        throw new ArgumentException($"不支持的端口类型 [{portType}]", nameof(portType));
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public static EthernetPortOptionsBuilder Tcp(IPEndPoint remote)
    {
        return Builder(TransportProtocol.Tcp, remote);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public static EthernetPortOptionsBuilder Udp(IPEndPoint remote)
    {
        return Builder(TransportProtocol.Udp, remote);
    }
}


/// <summary>
/// 以太网配置构建器
/// </summary>
public sealed class EthernetPortOptionsBuilder
{
    private TransportProtocol? _type;
    private IPEndPoint? _remote;
    private IPEndPoint? _local;


    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    internal EthernetPortOptionsBuilder SetType(TransportProtocol value)
    {
        if(value == TransportProtocol.Tcp || value == TransportProtocol.Udp)
        {
            _type = value;
        }
        else
        {
            throw new ArgumentException("不支持的端口类型", nameof(value));
        }

        return this;
    }
    public EthernetPortOptionsBuilder SetRemote(IPEndPoint value)
    {
        _remote = value;
        return this;
    }
    public EthernetPortOptionsBuilder SetLocal(IPEndPoint? value)
    {
        _local = value;
        return this;
    }


    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public EthernetPortOptions Build()
    {
        if (_type is null) throw new ArgumentNullException(nameof(_type), "端口类型不可为空");
        if (_remote is null) throw new ArgumentNullException(nameof(_remote), "远程地址不可为空");


        return new EthernetPortOptions()
        {
            Address = PortAddress.Create(_type, _remote.ToString()),
            Remote = _remote,
            Local = _local
        };
    }
}