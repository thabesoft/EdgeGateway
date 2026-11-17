namespace ThabeSoft.EdgeGateway.Configuration.Nodes;

public interface IConfigNode
{
    string? GetString(string path);
    IConfigNode? GetChild(string path);
    IEnumerable<IConfigNode> GetChildren(string path);
}
