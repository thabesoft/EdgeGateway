using Microsoft.Extensions.Time.Testing;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Devices;


[TestClass]
public class DefaultDeviceTests
{
    [TestMethod]
    public void TestMethod1()
    {

        var timeProvider = new FakeTimeProvider();
        var tag = ModbusTag.HoldingRegister(1, 0, WordCodec.BigEndian);
        var trigger = new FixedIntervalTrigger(TimeSpan.FromSeconds(1), timeProvider);

        //device.Register("test", tag, );

    }
}
