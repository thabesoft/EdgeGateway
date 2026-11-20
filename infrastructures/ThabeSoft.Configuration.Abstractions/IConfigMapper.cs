using ThabeSoft.Configuration.Nodes;

namespace ThabeSoft.Configuration;


public interface IConfigMapper<T>
{
    bool CanMap(IConfigNode root);
    T Map(IConfigNode root);
}
