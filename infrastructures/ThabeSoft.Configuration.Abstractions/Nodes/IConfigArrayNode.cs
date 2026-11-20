namespace ThabeSoft.Configuration.Nodes;


public interface IConfigArrayNode : IConfigNode, IReadOnlyList<IConfigNode>
{
    IConfigNode? GetByIndex(int index);
}
