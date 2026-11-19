using ThabeSoft.EdgeGateway.Configuration.Nodes;

namespace ThabeSoft.EdgeGateway.Configuration.Paths;

public interface IConfigPathEvaluator
{
    IConfigNode? Evaluate(IConfigNode start, string path);
    IConfigNode? Evaluate(IConfigNode start, IEnumerable<IConfigPathToken> tokens);
}
