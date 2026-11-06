namespace ThabeSoft.EdgeGateway.Subscription;


/// <summary>
/// 标签名称
/// </summary>
public sealed record class SubscriptionId : IEquatable<SubscriptionId>
{
    private readonly string _value;
    private SubscriptionId(string value) => _value = value;
    public override string ToString() => _value;


    public static SubscriptionId Create()
    {
        var guid = Guid.NewGuid().ToString("N");
        return new(guid);
    }
    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public static SubscriptionId From(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new(value.Trim());
    }
}