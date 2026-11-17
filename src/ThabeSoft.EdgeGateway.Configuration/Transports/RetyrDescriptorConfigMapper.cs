using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Transports;


public class RetyrDescriptorConfigMapper : IConfigMapper<RetryDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }
    public RetryDescriptor Map(IConfigNode root)
    {
        var startInterval = root.GetValue("StartInterval").Required().ToTimeSpan();
        var maxInterval = root.GetValue("MaxInterval").Required().ToTimeSpan();
        var backoffMultiplier = root.GetValue("BackoffMultiplier").Required().ToDouble();

        return new(startInterval, maxInterval) { BackoffMultiplier = backoffMultiplier };
    }
}