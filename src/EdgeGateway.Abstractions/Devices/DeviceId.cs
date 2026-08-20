namespace EdgeGateway.Devices;

public sealed record class DeviceId
{
    private readonly string _value;
    private DeviceId(string value) => _value = value;


    public static DeviceId New() => new(Guid.NewGuid().ToString("N"));

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static DeviceId FromString(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value), "协议类型不可为空");


        var trimmed = value.Trim();

        if (string.IsNullOrWhiteSpace(trimmed))
            throw new ArgumentException("DeviceId cannot be empty", nameof(value));

        return new(trimmed);
    }

    public override string ToString() => _value;
}