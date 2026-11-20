using ThabeSoft.EdgeGateway.Configuration.Nodes;

namespace ThabeSoft.EdgeGateway.Configuration;


public interface IConfigMapper<T>
{
    bool CanMap(IConfigNode root);
    T Map(IConfigNode root);
}