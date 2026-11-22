using System.Net;

namespace ThabeSoft.Transports;


public sealed record class ConnectionEndpoint : IEquatable<ConnectionEndpoint>
{
    public required TransportType TransportType { get; init; }
    public required string Address { get; init; }



    private ConnectionEndpoint() { }

    public bool Equals(ConnectionEndpoint? other)
    {
        if (other is null) return false;
        if (other.TransportType != TransportType) return false;

        return string.Equals(other?.Address, Address, StringComparison.OrdinalIgnoreCase);
    }
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(TransportType);
        hashCode.Add(StringComparer.OrdinalIgnoreCase.GetHashCode(Address));

        return hashCode.ToHashCode();
    }

    public override string ToString()
    {
        return $"{TransportType}/{Address}";
    }



    private static ConnectionEndpoint Create(TransportType connectionType, string endpoint)
    {
        if (!Enum.IsDefined(connectionType))
            throw new ArgumentOutOfRangeException(nameof(connectionType));

        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);


        return new() { TransportType = connectionType, Address = endpoint.Trim() };
    }
    public static ConnectionEndpoint Serial(string portName) =>
        Create(TransportType.Serial, portName);
    public static ConnectionEndpoint Ethernet(IPEndPoint endpoint) =>
        Create(TransportType.Ethernet, endpoint.ToString());
}