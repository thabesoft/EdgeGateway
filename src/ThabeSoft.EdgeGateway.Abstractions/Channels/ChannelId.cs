namespace ThabeSoft.EdgeGateway.Channels;


/// <summary>
/// 通道的唯一标识符
/// </summary>
public sealed record class ChannelId
{
    private readonly string _value;
    private ChannelId(string value) => _value = value;
    public override string ToString() => _value;



    public static ChannelId Create()
    {
        var guid = Guid.NewGuid().ToString("N");
        return new(guid);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public static ChannelId Parse(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new(value.Trim());
    }
}