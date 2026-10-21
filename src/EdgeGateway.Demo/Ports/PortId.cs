namespace EdgeGateway.Ports;


/// <summary>
/// 设备的唯一标识符
/// </summary>
public sealed record class PortId
{
    private readonly string _value;
    private PortId(string value) => _value = value;
    public override string ToString() => _value;



    public static PortId Create()
    {
        var guid = Guid.NewGuid().ToString("N");
        return new(guid);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public static PortId Parse(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new(value.Trim());
    }
}