using ThabeSoft.Configuration.Nodes;

namespace ThabeSoft.Configuration.Paths;


internal sealed class ConfigPathEvaluator(IConfigPathParser parser) : IConfigPathEvaluator
{
    public IConfigNode? Evaluate(IConfigNode start, string path)
    {
        var tokens = parser.Parse(path);
        return Evaluate(start, tokens);
    }

    public IConfigNode? Evaluate(IConfigNode start, IEnumerable<IConfigPathToken> tokens)
    {
        IConfigNode? current = start;
        foreach (var token in tokens)
        {
            if (current is null)
                break;
            current = token.Execute(current);
        }
        return current;
    }
}