using System.Text.Json.Nodes;

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
        GetRoot = root;
        GetParent = parent;
    }

    /// <summary>
    /// 根节点
    /// </summary>
    public JsonConfigNode(JsonNode content)
    {
        _content = content;
        GetRoot = this;
        GetParent = null;
    }


    public IConfigNode GetRoot { get; }
    public IConfigNode? GetParent { get; }
    public string GetPath => _content.GetPath();


    public IConfigNode? this[int index]
    {
        get
        {
            var jsonNode = _content[index];
            if (jsonNode is null) return null;

            return new JsonConfigNode(jsonNode, GetRoot, this);
        }
    }

    public IConfigNode? this[string path]
    {
        get
        {
            var jsonNode = _content[path];
            if (jsonNode is null) return null;

            return new JsonConfigNode(jsonNode, GetRoot, this);
        }
    }


    public string? AsString()
    {
        return _content.ToString();
    }
    public IEnumerable<IConfigNode> AsArray()
    {
        foreach (var i in _content.AsArray() ?? [])
        {
            if (i is null) continue;
            yield return new JsonConfigNode(i, GetRoot, this);
        }
    }

    public override string ToString()
    {
        return _content.ToString();
    }
}