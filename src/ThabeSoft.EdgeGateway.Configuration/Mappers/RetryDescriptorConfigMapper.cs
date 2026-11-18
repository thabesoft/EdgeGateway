using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Mappers;


public class RetryDescriptorConfigMapper : IConfigMapper<RetryDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }
    public RetryDescriptor Map(IConfigNode root)
    {
        var startInterval = root.GetStringValue("StartInterval").Required().ToTimeSpan();
        var maxInterval = root.GetStringValue("MaxInterval").Required().ToTimeSpan();
        var backoffMultiplier = root.GetStringValue("BackoffMultiplier").Required().ToDouble();

        return new(startInterval, maxInterval) { BackoffMultiplier = backoffMultiplier };
    }
}