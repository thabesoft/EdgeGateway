namespace ThabeSoft.EdgeGateway.Tags;


/// <summary>
/// 标签名称
/// </summary>
public sealed record class TagId : IEquatable<TagId>
{
    private readonly string _value;
    private TagId(string value) => _value = value;
    public override string ToString() => _value;


    public static TagId Create()
    {
        var guid = Guid.NewGuid().ToString("N");
        return new(guid);
    }
    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public static TagId From(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new(value.Trim());
    }
}