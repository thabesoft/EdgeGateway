namespace ThabeSoft.EdgeGateway.Configuration;


public interface IConfigNode
{
    string? GetValue(string path);
    IConfigNode? GetChild(string path);
    IEnumerable<IConfigNode> GetChildren(string path);
}