using FluentResults;

namespace EdgeGateway.Ports;

public interface IPortManager
{
    IReadOnlyDictionary<PortAddress, IPortHandle> Ports { get; }


    Result<IPortHandle> Add(IPortConfig config);
    ValueTask<Result> RemovAsync(PortAddress address, CancellationToken cancellationToken = default);
}