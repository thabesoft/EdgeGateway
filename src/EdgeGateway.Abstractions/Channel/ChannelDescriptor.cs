using EdgeGateway.Protocols;

namespace EdgeGateway.Bytes;


/// <summary>
/// 端口描述
/// </summary>
public record class ChannelDescriptor
{
    public required IChannelAddress Address { get; init; }
    public required ConnectionType ConnectionType { get; init; }
    public required ProtocolType Protocol { get; init; }


    private ChannelDescriptor() { }


   
    private static ChannelDescriptor Create(IChannelAddress address, ConnectionType connectionType, ProtocolType protocol)
    {
        ArgumentNullException.ThrowIfNull(address);
        if (!Enum.IsDefined(connectionType)) throw new ArgumentOutOfRangeException(nameof(connectionType));
        ArgumentNullException.ThrowIfNull(protocol);

        return new ChannelDescriptor()
        {
            Address = address,
            ConnectionType = connectionType,
            Protocol = protocol
        };
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public static ChannelDescriptor Serial(SerialChannelAddress address, ProtocolType protocol) =>
        Create(address, ConnectionType.Serial, protocol);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public static ChannelDescriptor Tcp(IpChannelAddress address, ProtocolType protocol) =>
        Create(address, ConnectionType.Tcp, protocol);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    public static ChannelDescriptor Udp(IpChannelAddress address, ProtocolType protocol) =>
        Create(address, ConnectionType.Udp, protocol);
}


public enum ConnectionType
{
    Serial,
    Tcp,
    Udp,
}