using System.Text.Json.Nodes;
using ThabeSoft.EdgeGateway.Configuration.Json.Nodes;
using ThabeSoft.EdgeGateway.Configuration.Nodes;
using ThabeSoft.EdgeGateway.Configuration.Paths;

namespace ThabeSoft.EdgeGateway.Configuration.Json;

internal sealed class JsonConfigNodeParser(IConfigPathEvaluator configPathEvaluator) : IConfigNodeParser
{
    public async ValueTask<IConfigNode> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var root = await JsonNode.ParseAsync(stream, cancellationToken: cancellationToken);

        if (root is JsonArray arr)
        {
            return new JsonArrayConfigNode(arr, configPathEvaluator);
        }
        else if (root is JsonObject obj)
        {
            return new JsonObjectConfigNode(obj, configPathEvaluator);
        }
        else if (root is JsonValue value)
        {
            return new JsonValuConfigNode(value, configPathEvaluator);
        }

        throw new InvalidOperationException("不支持的配置格式");
    }
}