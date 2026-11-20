using System.Text.Json.Nodes;
using ThabeSoft.Configuration.Json.Nodes;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.Configuration.Paths;

namespace ThabeSoft.Configuration.Json;


internal sealed class JsonValuConfigNode(
        JsonValue content,
        IConfigPathEvaluator configPathEvaluator
    ) : JsonConfigNode(configPathEvaluator), IConfigValueNode
{
    public override ConfigNodeType Type => ConfigNodeType.Value;

    public string? GetString()
    {
        return content.ToString();
        //return content.GetValue<string>();
    }


    public override string ToString()
    {
        return content.ToString();
    }
}
