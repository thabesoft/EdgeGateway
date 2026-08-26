using EdgeGateway.Dto;
using FluentResults;

namespace EdgeGateway.Serialization;


public interface IGatewayOptionsSerializer
{
    ValueTask<Result<GatewayOptionsDto>> DeserializeAsync(Stream source, CancellationToken cancellationToken = default);
    ValueTask<Result> SerializeAsync(Stream destination, GatewayOptionsDto options, CancellationToken cancellationToken = default);
}