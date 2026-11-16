using System.Text.Json.Nodes;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration;


internal sealed class JsonConfigSerializer : IConfigSerializer
{
    public async ValueTask<GatewayConfig> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var root = await JsonNode.ParseAsync(stream, cancellationToken: cancellationToken);
        return root.Required("网关配置不可为空").AsGatewayConfig();
    }

    public ValueTask SerializeAsync(GatewayConfig config, Stream stream, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
