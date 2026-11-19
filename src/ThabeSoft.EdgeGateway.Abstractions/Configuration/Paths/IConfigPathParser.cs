namespace ThabeSoft.EdgeGateway.Configuration.Paths;

public interface IConfigPathParser
{
    IEnumerable<IConfigPathToken> Parse(string path);
}
