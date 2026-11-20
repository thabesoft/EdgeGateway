using System.Collections;
using System.Text.Json.Nodes;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.Configuration.Paths;

namespace ThabeSoft.Configuration.Json.Nodes;


internal sealed class JsonArrayConfigNode(
        JsonArray content,
        IConfigPathEvaluator configPathEvaluator
    ) : JsonConfigNode(configPathEvaluator), IConfigArrayNode
{
    public override ConfigNodeType Type => ConfigNodeType.Array;
    public int Count => content.Count;
    public IConfigNode this[int index] => GetByIndex(index)!;


    public IConfigNode? GetByIndex(int index)
    {
        var jsonNode = content[index];
        if (jsonNode is null) return null;

        return TryCreateNode(jsonNode);
    }

    public IEnumerator<IConfigNode> GetEnumerator()
    {
        int index = 0;

        foreach (var item in content)
        {
            var node = TryCreateNode(item, index++);
            if (node is null) continue;

            yield return node;
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
