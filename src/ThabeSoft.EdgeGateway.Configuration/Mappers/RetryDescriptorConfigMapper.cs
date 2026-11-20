using ThabeSoft.EdgeGateway.Configuration.Nodes;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Configuration.Mappers;


public class RetryDescriptorConfigMapper : IConfigMapper<RetryDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }
    public RetryDescriptor Map(IConfigNode root)
    {
        var startInterval = root.GetStringByPath("StartInterval").Required().ToTimeSpan();
        var maxInterval = root.GetStringByPath("MaxInterval").Required().ToTimeSpan();
        var backoffMultiplier = root.GetStringByPath("BackoffMultiplier").Required().ToDouble();

        return new(startInterval, maxInterval) { BackoffMultiplier = backoffMultiplier };
    }
}