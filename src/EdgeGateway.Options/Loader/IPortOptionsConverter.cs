using EdgeGateway.Ports;

namespace EdgeGateway.Options.Loader;


public interface IPortOptionsConverter
{
    bool CanConvert(PortOptionsDto dto);
    IPortOptions Convert(PortOptionsDto source);
}