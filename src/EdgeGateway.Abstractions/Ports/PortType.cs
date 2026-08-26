using System.Diagnostics.CodeAnalysis;

namespace EdgeGateway.Ports;


public sealed record class PortType
#if !NETSTANDARD2_0
    : ISpanParsable<PortType>
#endif
{
    public static PortType SerialPort { get; } = Parse(nameof(SerialPort));
    public static PortType Tcp { get; } = Parse(nameof(Tcp));
    public static PortType Udp { get; } = Parse(nameof(Udp));



    private readonly string _value;
    private PortType(string value) => _value = value;


    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static PortType Parse(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value), "协议类型不可为空");

        var trimmed = value.Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) throw new ArgumentException("协议类型不可为空白", nameof(value));

        return new PortType(trimmed);
    }
    public static bool TryParse(string value, [NotNullWhen(true)] out PortType? protocol)
    {
        protocol = null;

        var trimmed = value.Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) return false;

        protocol = new PortType(trimmed);
        return true;
    }

    public static implicit operator string(PortType portType) => portType._value;


    public override string ToString() => _value;
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(_value);
    public bool Equals(PortType? other) => _value.Equals(other?._value, StringComparison.OrdinalIgnoreCase);




    public static PortType Parse(ReadOnlySpan<char> s, IFormatProvider? _)
    {
        var trimmed = s.Trim();
        if (trimmed.IsWhiteSpace()) throw new ArgumentException("协议类型不可为空白", nameof(s));

        return new PortType(trimmed.ToString());
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? _, [MaybeNullWhen(false)] out PortType result)
    {
        result = null;

        var trimmed = s.Trim();
        if (trimmed.IsWhiteSpace()) return false;

        result = new PortType(trimmed.ToString());
        return true;
    }

    public static PortType Parse(string s, IFormatProvider? provider)
        => Parse(s.AsSpan(), provider);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out PortType result)
        => TryParse(s.AsSpan(), provider, out result);
}