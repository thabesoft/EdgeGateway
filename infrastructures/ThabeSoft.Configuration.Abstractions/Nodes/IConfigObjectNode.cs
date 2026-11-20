namespace ThabeSoft.Configuration.Nodes;


public interface IConfigObjectNode : IConfigNode, IEnumerable<KeyValuePair<string, IConfigNode>>
{
    IConfigNode? GetByName(string name);
}
