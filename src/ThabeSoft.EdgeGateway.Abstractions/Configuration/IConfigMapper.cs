namespace ThabeSoft.EdgeGateway.Configuration;


public interface IConfigMapper<T>
{
    bool CanMap(IConfigNode root);
    T Map(IConfigNode root);
}