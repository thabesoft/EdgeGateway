namespace ThabeSoft.Configuration.Paths;

public interface IConfigPathParser
{
    IEnumerable<IConfigPathToken> Parse(string path);
}
