using ThabeSoft.Configuration.Nodes;

namespace ThabeSoft.Configuration.Paths;

public interface IConfigPathEvaluator
{
    IConfigNode? Evaluate(IConfigNode start, string path);
    IConfigNode? Evaluate(IConfigNode start, IEnumerable<IConfigPathToken> tokens);
}
