using System.Net;

namespace EdgeGateway.Bytes;


/// <summary>
/// 通讯端点地址
/// </summary>
public interface IChannelAddress : IEquatable<IChannelAddress>;


/// <summary>
/// Ip通讯端点地址
/// </summary>
public sealed record class IpChannelAddress : IChannelAddress
{
    public required IPAddress Address { get; init; }
    public required int Port { get; init; }


    private IpChannelAddress() { }
    public bool Equals(IChannelAddress? other) => Equals(other as IpChannelAddress);



    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public static IpChannelAddress Create(IPAddress iPAddress, int port)
    {
        ArgumentNullException.ThrowIfNull(iPAddress);

        if (port is < 0 or > 65535)
            throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 0 and 65535.");

        return new IpChannelAddress() { Address = iPAddress, Port = port };
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public static IpChannelAddress Create(IPEndPoint iPEndPoint) =>
        Create(iPEndPoint.Address, iPEndPoint.Port);

    public static IpChannelAddress Parse(ReadOnlySpan<char> iPEndPointString) =>
        Create(IPEndPoint.Parse(iPEndPointString));
}

/// <summary>
/// 串口通讯端点地址
/// </summary>
public sealed record class SerialChannelAddress : IChannelAddress, IEquatable<SerialChannelAddress>
{
    public required string PortName { get; init; }


    private SerialChannelAddress() { }
    public bool Equals(IChannelAddress? other) => other is SerialChannelAddress serialPortAddress && string.Equals(serialPortAddress.PortName, PortName, StringComparison.OrdinalIgnoreCase);
    public bool Equals(SerialChannelAddress? other) => string.Equals(other?.PortName, PortName, StringComparison.OrdinalIgnoreCase);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(PortName);



    /// <inheritdoc/>
    /// <exception cref="ArgumentException"/>
    public static SerialChannelAddress Create(string portName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(portName);
        return new SerialChannelAddress() { PortName = portName.Trim() };
    }
}