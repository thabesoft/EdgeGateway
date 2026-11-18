using System.Text.Json.Nodes;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Json;


internal sealed class JsonConfigNode : IConfigNode
{
    private readonly JsonNode _content;

    /// <summary>
    /// 子节点
    /// </summary>
    public JsonConfigNode(JsonNode content, IConfigNode root, IConfigNode? parent = null)
    {
        _content = content;
        Root = root;
        Parent = parent;
    }

    /// <summary>
    /// 根节点
    /// </summary>
    public JsonConfigNode(JsonNode content)
    {
        _content = content;
        Root = this;
        Parent = null;
    }


    public IConfigNode Root { get; }
    public IConfigNode? Parent { get; }
    public IConfigNode? this[string path]
    {
        get
        {
            var jsonNode = GetByPath(path);
            if (jsonNode is null) return null;

            return new JsonConfigNode(jsonNode, Root, this);
        }
    }


    public string? AsString()
    {
        return _content.AsValue().ToString();
    }
    public IEnumerable<IConfigNode> AsArray()
    {
        foreach (var i in _content.AsArray() ?? [])
        {
            if (i is null) continue;
            yield return new JsonConfigNode(i, Root, this);
        }
    }



    private JsonNode? GetByPath(string pathString)
    {
        var keys = pathString.Split(':', StringSplitOptions.RemoveEmptyEntries);

        var currentNode = _content;
        foreach (var key in keys)
        {
            currentNode = currentNode[key];
            if (currentNode is null) return null;
        }

        return currentNode;
    }
}