namespace ThabeSoft.Configuration.Nodes;


public interface IConfigValueNode : IConfigNode
{
    string? GetString();
}