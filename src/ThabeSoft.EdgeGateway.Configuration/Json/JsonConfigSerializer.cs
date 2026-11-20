using ThabeSoft.Configuration.Models;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.Configuration.Json;


internal sealed class JsonConfigSerializer(
        IConfigNodeParser parser,
        IConfigMapper<GatewayConfig> mapper
    ) : IConfigSerializer
{
    public async ValueTask<GatewayConfig> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var root = await parser.ParseAsync(stream, cancellationToken: cancellationToken);
        return mapper.Map(root.Required("网关配置不可为空"));
    }

    public ValueTask SerializeAsync(GatewayConfig config, Stream stream, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}