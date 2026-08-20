using System.Diagnostics.CodeAnalysis;

namespace EdgeGateway.Ports;


/// <summary>
/// 设备地址 包含端口类型和地址信息
/// </summary>
public sealed record class PortAddress
{
    public PortType PortType { get; }
    public string Location { get; }


    private PortAddress(PortType type, string location)
    {
        PortType = type;
        Location = location;
    }


    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static PortAddress Create(PortType portType, string location)
    {
        if (location is null) throw new ArgumentNullException(nameof(location), "位置不能为空");

        var trimmedValue = location.Trim();
        if (string.IsNullOrWhiteSpace(trimmedValue))
            throw new ArgumentException("位置不能为空", nameof(location));

        return new PortAddress(portType, trimmedValue);
    }
    public static bool TryCreate(PortType portType, string location, [NotNullWhen(true)] out PortAddress? address)
    {
        address = null;
        var trimmedValue = location.Trim();

        if (string.IsNullOrWhiteSpace(trimmedValue)) return false;

        address = new PortAddress(portType, trimmedValue);
        return true;
    }


    /// <summary>
    /// 返回端口地址的字符串表示。
    /// </summary>
    /// <returns>
    /// 格式为 <c>PortType[Location]</c>，例如：
    /// <list type="bullet">
    /// <item><description><c>SerialPort[COM3]</c></description></item>
    /// <item><description><c>Tcp[192.168.1.100:502]</c></description></item>
    /// <item><description><c>Udp[192.168.1.100:502]</c></description></item>
    /// </list>
    /// </returns>
    public override string ToString()
    {
        return $"{PortType}[{Location}]";
    }

    public bool Equals(PortAddress? other)
    {
        return PortType.Equals(other?.PortType)
            && string.Equals(other?.Location, Location, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
#if NETSTANDARD2_0
        unchecked
        {
            int hash = 17;
            hash = (hash * 31) + PortType.GetHashCode();
            hash = (hash * 31) + StringComparer.OrdinalIgnoreCase.GetHashCode(Location ?? string.Empty);
            return hash;
        }
#else
        return HashCode.Combine(
            PortType.GetHashCode(),
            StringComparer.OrdinalIgnoreCase.GetHashCode(Location)
        );
#endif
    }
}