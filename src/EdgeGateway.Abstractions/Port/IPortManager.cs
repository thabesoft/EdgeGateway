namespace EdgeGateway.Port;


public interface IPortManager
{
    IReadOnlyDictionary<PortId, IPort> Ports { get; }

    IPort AddPort(IPortDescriptor descriptor);
    ValueTask RemovePortAsync(PortId id, CancellationToken cancellationToken = default);
}