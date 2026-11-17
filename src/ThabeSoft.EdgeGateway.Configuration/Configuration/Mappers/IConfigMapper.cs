using ThabeSoft.EdgeGateway.Configuration.Nodes;

namespace ThabeSoft.EdgeGateway.Configuration.Mappers;


public interface IConfigMapper<T>
{
    T Map(IConfigNode root);
}
