namespace ThabeSoft.EdgeGateway.Configuration.Nodes;


public interface IConfigValueNode : IConfigNode
{
    string? GetString();
}