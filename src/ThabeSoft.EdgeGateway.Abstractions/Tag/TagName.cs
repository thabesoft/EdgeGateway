namespace EdgeGateway.Tag;


/// <summary>
/// 标签名称
/// </summary>
public sealed record class TagName : IEquatable<TagName>
{
    private readonly string _value;
    private TagName(string value) => _value = value;
    public bool Equals(TagName? other) => string.Equals(other?._value, _value, StringComparison.OrdinalIgnoreCase);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(_value);
    public override string ToString() => _value;


    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public static TagName From(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new(value.Trim());
    }
}