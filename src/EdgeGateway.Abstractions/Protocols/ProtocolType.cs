using System.Diagnostics.CodeAnalysis;

namespace EdgeGateway.Protocols;


/// <summary>
/// 通信协议类型
/// </summary>
public sealed record class ProtocolType
{
    public static ProtocolType ModbusRtu { get; } = Parse(nameof(ModbusRtu));
    public static ProtocolType ModbusAscii { get; } = Parse(nameof(ModbusAscii));
    public static ProtocolType ModbusIp { get; } = Parse(nameof(ModbusIp));



    private readonly string _value;
    private ProtocolType(string value) => _value = value;


    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static ProtocolType Parse(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value), "协议类型不可为空");

        var trimmed =  value.Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) throw new ArgumentException("协议类型不可为空白", nameof(value));

        return new ProtocolType(trimmed);
    }
    public static bool TryParse(string value, [NotNullWhen(true)] out ProtocolType? protocol )
    {
        protocol = null;

        var trimmed = value.Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) return false;

        protocol = new ProtocolType(trimmed);
        return true;
    }


    public static implicit operator string(ProtocolType protocolType) => protocolType._value;


    public override string ToString() => _value;
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(_value);
    public bool Equals(ProtocolType? other) => _value.Equals(other?._value, StringComparison.OrdinalIgnoreCase);
}