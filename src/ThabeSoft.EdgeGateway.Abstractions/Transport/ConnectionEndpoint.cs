using System.Net;

namespace ThabeSoft.EdgeGateway.Transport;


public sealed record class ConnectionEndpoint : IEquatable<ConnectionEndpoint>
{
    public required ConnectionType Type { get; init; }
    public required string Address { get; init; }



    private ConnectionEndpoint() { }

    public bool Equals(ConnectionEndpoint? other)
    {
        if (other is null) return false;
        if (other.Type != Type) return false;

        return string.Equals(other?.Address, Address, StringComparison.OrdinalIgnoreCase);
    }
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(Type);
        hashCode.Add(StringComparer.OrdinalIgnoreCase.GetHashCode(Address));

        return hashCode.ToHashCode();
    }

    public override string ToString()
    {
        return $"{Type}/{Address}";
    }



    private static ConnectionEndpoint Create(ConnectionType connectionType, string endpoint)
    {
        if (!Enum.IsDefined(connectionType))
            throw new ArgumentOutOfRangeException(nameof(connectionType));

        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);


        return new() { Type = connectionType, Address = endpoint.Trim() };
    }
    public static ConnectionEndpoint Serial(string portName) =>
        Create(ConnectionType.Serial, portName);
    public static ConnectionEndpoint Ethernet(IPEndPoint endpoint) =>
        Create(ConnectionType.Ethernet, endpoint.ToString());
}