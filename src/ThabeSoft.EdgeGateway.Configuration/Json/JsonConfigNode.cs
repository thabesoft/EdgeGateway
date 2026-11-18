using System.Text.Json.Nodes;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Json;


internal sealed class JsonConfigNode(JsonNode node) : IConfigNode
{
    public IConfigNode? GetChild(string path)
    {
        var jsonNode = GetByPath(path);
        if (jsonNode is null) return null;

        return new JsonConfigNode(jsonNode);
    }

    public IEnumerable<IConfigNode> GetChildren(string path)
    {
        foreach (var i in GetByPath(path)?.AsArray() ?? [])
        {
            if (i is null) continue;
            yield return new JsonConfigNode(i);
        }
    }

    public string? GetValue(string path)
    {
        return GetByPath(path)?.ToString();
    }

    private JsonNode? GetByPath(string path)
    {
        var paths = path.Split(':', StringSplitOptions.RemoveEmptyEntries);

        var current = node;
        foreach (var i in paths)
        {
            current = node[i];
            if (current is null) return null;
        }

        return current;
    }
}


internal sealed class JsonConfigNodeParser : IConfigNodeParser
{
    public async ValueTask<IConfigNode> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var root = await JsonNode.ParseAsync(stream, cancellationToken: cancellationToken);
        return new JsonConfigNode(root.Required("网关配置不可为空"));
    }
}