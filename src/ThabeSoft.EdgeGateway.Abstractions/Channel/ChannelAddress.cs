namespace ThabeSoft.EdgeGateway.Channel;


public sealed record class ChannelAddress : IEquatable<ChannelAddress>
{
    public required ConnectionType ConnectionType { get; init; }
    public required string Endpoint { get; init; }



    private ChannelAddress() { }

    public bool Equals(ChannelAddress? other)
    {
        if (other is null) return false;
        if (other.ConnectionType != ConnectionType) return false;

        return string.Equals(other?.Endpoint, Endpoint, StringComparison.OrdinalIgnoreCase);
    }
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(ConnectionType);
        hashCode.Add(StringComparer.OrdinalIgnoreCase.GetHashCode(Endpoint));

        return hashCode.ToHashCode();
    }

    public override string ToString()
    {
        return $"{ConnectionType}/{Endpoint}";
    }



    public static ChannelAddress Create(ConnectionType connectionType, string endpoint)
    {
        if (!Enum.IsDefined(connectionType))
            throw new ArgumentOutOfRangeException(nameof(connectionType));

        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);


        return new() { ConnectionType = connectionType, Endpoint = endpoint.Trim() };
    }
}