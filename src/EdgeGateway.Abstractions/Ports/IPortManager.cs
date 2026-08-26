using FluentResults;

namespace EdgeGateway.Ports;


public interface IPortManager
{
    IReadOnlyDictionary<PortAddress, IPortHandle> Ports { get; }

    Result<IPortHandle> Add(IPortOptions options);
    ValueTask<Result> RemoveAsync(PortAddress address, CancellationToken cancellationToken = default);
}