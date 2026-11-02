using Moq;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Polling;


[TestClass]
public class PollingSchedulerTests
{
    [TestMethod]
    public void Register_Should_SubscribeToTrigger()
    {
        var mockTagReader = new Mock<ITagReader>();
        var pollingScheduler = new PollingScheduler(mockTagReader.Object);

        var mockTag = new Mock<ITag<int>>();
        var mockTrigger = new Mock<ITrigger>();

        _ = pollingScheduler.Register("test,", mockTag.Object, mockTrigger.Object);


        Assert.HasCount(1, pollingScheduler.Monitors);
        mockTrigger.Verify(t => t.Subscribe(It.IsAny<TriggerHandler>()), Times.Once,
            "注册后，Scheduler 应该向 Trigger 订阅回调");
    }


    [TestMethod]
    public void Unregister_Should_UnsubscribeFromTrigger()
    {
        var mockTagReader = new Mock<ITagReader>();
        var pollingScheduler = new PollingScheduler(mockTagReader.Object);

        var mockTag = new Mock<ITag<int>>();
        var mockTrigger = new Mock<ITrigger>();

        var tagMonitor = pollingScheduler.Register("test", mockTag.Object, mockTrigger.Object);
        pollingScheduler.Unregister(tagMonitor.Name);


        Assert.HasCount(0, pollingScheduler.Monitors);
    }
}
