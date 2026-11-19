using System.Text.Json.Nodes;
using ThabeSoft.EdgeGateway.Configuration.Nodes;
using ThabeSoft.EdgeGateway.Configuration.Paths;

namespace ThabeSoft.EdgeGateway.Configuration.Json.Nodes;


/// <summary>
/// 子节点
/// </summary>
internal abstract class JsonConfigNode(IConfigPathEvaluator pathEvaluator) : IConfigNode
{
    public abstract ConfigNodeType Type { get; }
    public JsonConfigNode Root { get => field ?? this; init; }
    public JsonConfigNode? Parent { get; init; }
    public object? LocationKey { get; init; }

    IConfigNode IConfigNode.Root => Root;
    IConfigNode? IConfigNode.Parent => Parent;


    public IConfigNode? GetByPath(string path)
    {
        return pathEvaluator.Evaluate(this, path);
    }
    public string GetPath()
    {
        var parts = new List<string>();
        JsonConfigNode? current = this;

        while (current != null)
        {
            var key = current.LocationKey;
            if (key is null) break; // root，停止
            if (key is string name)
            {
                parts.Add(name);
            }
            else if (key is int idx)
            {
                parts.Add($"[{idx}]");
            }
            current = current.Parent;
        }

        parts.Reverse();
        return "$" + string.Concat(parts);
    }

    protected IConfigNode TryCreateNode(JsonNode? node, object? locationKey = null)
    {
        if (node is JsonArray arr)
        {
            return new JsonArrayConfigNode(arr, pathEvaluator) { Parent = this, Root = Root, LocationKey = locationKey };
        }
        if (node is JsonObject obj)
        {
            return new JsonObjectConfigNode(obj, pathEvaluator) { Parent = this, Root = Root, LocationKey = locationKey };
        }
        if (node is JsonValue value)
        {
            return new JsonValuConfigNode(value, pathEvaluator) { Parent = this, Root = Root, LocationKey = locationKey };
        }

        throw new InvalidOperationException($"不支持的 JsonNode 类型：{node?.GetType().Name}");
    }
}