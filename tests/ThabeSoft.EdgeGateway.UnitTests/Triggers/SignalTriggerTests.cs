using ThabeSoft.EdgeGateway.Triggers.Signal;

namespace ThabeSoft.EdgeGateway.Triggers;

[TestClass]
public class SignalTriggerTests
{
    [TestMethod]
    public async Task WaitForNextTickAsync()
    {
        var factory = new SignalTriggerFactory();
        var trigger = factory.Create(Trigger.Signal(out var signal));

        // 1. 发起等待，拿到未完成的 Task
        var task = trigger.WaitForNextTickAsync(TestContext.CancellationToken);

        // 2. 断言：未 Signal 前不完成
        Assert.IsFalse(task.IsCompleted);

        // 3. 触发信号
        signal.OnSignal();

        // 4. 等待完成（并隐式断言没抛异常/没挂起）
        await task;
    }


    public TestContext TestContext { get; set; }
}
