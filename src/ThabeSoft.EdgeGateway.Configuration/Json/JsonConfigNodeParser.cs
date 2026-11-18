using System.Text.Json.Nodes;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Json;

internal sealed class JsonConfigNodeParser : IConfigNodeParser
{
    public async ValueTask<IConfigNode> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var root = await JsonNode.ParseAsync(stream, cancellationToken: cancellationToken);
        return new JsonConfigNode(root.Required("网关配置不可为空"));
    }
}