using System.Diagnostics.CodeAnalysis;

namespace EdgeGateway.Protocols;


/// <summary>
/// 通信协议类型
/// </summary>
public readonly record struct ProtocolType
{
    private readonly string _value;
    private ProtocolType(string value) => _value = value;


    public override string ToString() => _value;
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(_value);
    public bool Equals(ProtocolType? other) => string.Equals(_value, other?._value, StringComparison.OrdinalIgnoreCase);




    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public static ProtocolType Parse(ReadOnlySpan<char> type)
    {
        var trimmed = type.Trim();
        if (trimmed.IsWhiteSpace()) throw new ArgumentException("协议类型不可为空白", nameof(type));

        return new ProtocolType(trimmed.ToString());
    }
    public static bool TryParse(ReadOnlySpan<char> type, [MaybeNullWhen(false)] out ProtocolType result)
    {
        result = default;

        var trimmed = type.Trim();
        if (trimmed.IsWhiteSpace()) return false;

        result = new ProtocolType(trimmed.ToString());
        return true;
    }
}