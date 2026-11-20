using System.Collections;
using System.Text.Json.Nodes;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.Configuration.Paths;

namespace ThabeSoft.Configuration.Json.Nodes;


internal sealed class JsonObjectConfigNode(
        JsonObject content,
        IConfigPathEvaluator configPathEvaluator
    ) : JsonConfigNode(configPathEvaluator), IConfigObjectNode
{
    public override ConfigNodeType Type => ConfigNodeType.Object;


    public IConfigNode? GetByName(string name)
    {
        if (Type != ConfigNodeType.Object)
            throw new InvalidOperationException($"该节点 ({Type}) 无法通过名称 ({name}) 访问: {GetPath()}");

        var jsonNode = content[name];
        if (jsonNode is null) return null;

        return TryCreateNode(jsonNode, name);
    }

    public IEnumerator<KeyValuePair<string, IConfigNode>> GetEnumerator()
    {
        foreach (var (name, json) in content)
        {
            if (json is null) continue;

            var node = TryCreateNode(json, name);
            yield return new KeyValuePair<string, IConfigNode>(name, node);
        }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return content.ToString();
    }
}
