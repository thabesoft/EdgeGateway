using ThabeSoft.Configuration;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.EdgeGateway.Configuration.Models;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Configuration;


internal sealed class GatewayJsonConfigSerializer(
        IConfigNodeParser parser,
        IConfigMapper<GatewayConfig> mapper
    ) : IConfigSerializer
{
    public async ValueTask<object> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var root = await parser.ParseAsync(stream, cancellationToken: cancellationToken);
        return mapper.Map(root.Required("网关配置不可为空"));
    }

    public ValueTask SerializeAsync(object config, Stream stream, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}