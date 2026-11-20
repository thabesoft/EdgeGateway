using ThabeSoft.EdgeGateway.Configuration.Nodes;

namespace ThabeSoft.EdgeGateway.Configuration.Paths;

public interface IConfigPathToken
{
    ConfigPathType Type { get; }
    IConfigNode? Execute(IConfigNode node);
}


public sealed class ConfigNamePathToken(string name) : IConfigPathToken
{
    public ConfigPathType Type => ConfigPathType.Name;
    public IConfigNode? Execute(IConfigNode node) => node.AsObject()?.GetByName(name);
}

public sealed class ConfigIndexPathToken(int index) : IConfigPathToken
{
    public ConfigPathType Type => ConfigPathType.Index;
    public IConfigNode? Execute(IConfigNode node) => node.AsArray()?.GetByIndex(index);
}

public sealed class ConfigParentPathToken : IConfigPathToken
{
    public ConfigPathType Type => ConfigPathType.Parent;
    public IConfigNode? Execute(IConfigNode node) => node.Parent;
}

public sealed class ConfigRootPathToken : IConfigPathToken
{
    public ConfigPathType Type => ConfigPathType.Root;
    public IConfigNode? Execute(IConfigNode node) => node.Root;
}