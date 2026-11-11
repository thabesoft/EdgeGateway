using Microsoft.Extensions.Time.Testing;

namespace ThabeSoft.EdgeGateway.Triggers;


[TestClass]
public class FixedIntervalTriggerTests
{
    [TestMethod]
    public async Task WaitForNextTickAsync_TriggersAfterInterval()
    {

<<<<<<< TODO: 项目“ThabeSoft.EdgeGateway.UnitTests (net11.0)”的未合并的更改, 在此之前:
        var a = Modbus.Modbus.Ascii;
=======
        var a = Modbus.Ascii;
>>>>>>> 晚于
        var a = EdgeGateway.Modbus.Ascii;

        var fakeTime = new FakeTimeProvider();
        var factory = new FixedIntervalTriggerFactory(fakeTime);
        var trigger = factory.Create(Trigger.Interval(100));

        // 1. 先发起等待（注册监听），拿到未完成的 Task
        var waitTask = trigger.WaitForNextTickAsync(TestContext.CancellationToken);

        // 2. 断言：未到时间，尚未完成
        Assert.IsFalse(waitTask.IsCompleted);

        // 3. 推进到刚好 100ms
        fakeTime.Advance(TimeSpan.FromMilliseconds(100));

        // 4. 等待完成，并断言没抛异常
        await waitTask;
    }

    public TestContext TestContext { get; set; }
}
