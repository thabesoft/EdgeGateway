namespace ThabeSoft.EdgeGateway.Device;


/// <summary>
/// 设备的唯一标识符
/// </summary>
public sealed record class DeviceId
{
    private readonly string _value;
    private DeviceId(string value) => _value = value;
    public override string ToString() => _value;



    public static DeviceId Create()
    {
        var guid = Guid.NewGuid().ToString("N");
        return new(guid);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    public static DeviceId Parse(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new(value.Trim());
    }
}