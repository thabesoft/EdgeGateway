using System.Diagnostics.CodeAnalysis;

namespace EdgeGateway.Protocols;


/// <summary>
/// 通信协议类型
/// </summary>
public sealed record class ProtocolType
#if !NETSTANDARD2_0
    : ISpanParsable<ProtocolType>
#endif
{
    



    private readonly string _value;
    private ProtocolType(string value) => _value = value;

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public static ProtocolType Parse(ReadOnlySpan<char> s, IFormatProvider? _)
    {
        var trimmed = s.Trim();
        if (trimmed.IsWhiteSpace()) throw new ArgumentException("协议类型不可为空白", nameof(s));

        return new ProtocolType(trimmed.ToString());
    }
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? _, [MaybeNullWhen(false)] out ProtocolType result)
    {
        result = default;

        var trimmed = s.Trim();
        if (trimmed.IsWhiteSpace()) return false;

        result = new ProtocolType(trimmed.ToString());
        return true;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public static ProtocolType Parse(string s, IFormatProvider? provider = null)
        => Parse(s.AsSpan(), provider);
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ProtocolType result)
        => TryParse(s.AsSpan(), provider, out result);


    public override string ToString() => _value;
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(_value);
    public bool Equals(ProtocolType? other) => _value.Equals(other?._value, StringComparison.OrdinalIgnoreCase);
}