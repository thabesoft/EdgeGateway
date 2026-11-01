namespace ThabeSoft.EdgeGateway.Triggers;

[TestClass]
public class SignalTriggerTests
{
    [TestMethod]
    public async Task OnTick_ShouldInvokeHandlers_EveryTimeCalled()
    {
        var trigegr = new SignalTrigger();

        int count = 0;
        trigegr.Subscribe(_ =>
        {
            count++;
            return ValueTask.CompletedTask;
        });

        await trigegr.OnTickAsync(TestContext.CancellationToken);
        Assert.AreEqual(1, count);

        await trigegr.OnTickAsync(TestContext.CancellationToken);
        Assert.AreEqual(2, count);
    }

    [TestMethod]
    public async Task Unsubscribe_ShouldStopInvokingHandler()
    {
        var trigger = new SignalTrigger();
        int count = 0;

        var subscription = trigger.Subscribe(_ => { count++; return ValueTask.CompletedTask; });

        await trigger.OnTickAsync(TestContext.CancellationToken);
        Assert.AreEqual(1, count);

        subscription.Dispose();

        await trigger.OnTickAsync(TestContext.CancellationToken);
        Assert.AreEqual(1, count, "退订后不应该再执行回调");
    }

    [TestMethod]
    public async Task OnTick_ShouldInvokeAllHandlers()
    {
        var trigger = new SignalTrigger();
        var executionOrder = new List<int>();

        trigger.Subscribe(_ => { executionOrder.Add(1); return ValueTask.CompletedTask; });
        trigger.Subscribe(_ => { executionOrder.Add(2); return ValueTask.CompletedTask; });

        await trigger.OnTickAsync(TestContext.CancellationToken);

        Assert.AreSequenceEqual(new[] { 1, 2 }, executionOrder);
    }


    [TestMethod]
    public async Task OnTick_WhenSubscribeDuringExecution_ShouldNotThrowAndUseSnapshot()
    {
        // 1. 准备
        var trigger = new SignalTrigger();
        var tcs = new TaskCompletionSource();
        int firstHandlerInvokedCount = 0;
        int secondHandlerInvokedCount = 0;

        // 注册第一个 Handler，它会卡在 tcs.Task 上，模拟执行时间长的任务
        trigger.Subscribe(async _ =>
        {
            firstHandlerInvokedCount++;
            await tcs.Task;
        });

        // 2. 开始 Tick（此时会卡在第一个 Handler，处于 await 状态）
        var tickTask = trigger.OnTickAsync(TestContext.CancellationToken);

        // 3. 关键操作：趁 Tick 还没执行完，往集合里加一个新的 Handler
        // 如果底层没有使用快照（snapshot），这行代码会导致 tickTask 里的 foreach 抛出 Collection was modified 异常
        trigger.Subscribe(_ =>
        {
            secondHandlerInvokedCount++;
            return ValueTask.CompletedTask;
        });

        // 4. 放行第一个 Handler
        tcs.SetResult();

        // 5. 等待 Tick 完成
        await tickTask;

        // 6. 验证
        Assert.AreEqual(1, firstHandlerInvokedCount, "第一个 Handler 应该被执行");
        Assert.AreEqual(0, secondHandlerInvokedCount, "第二个 Handler 是在 Tick 过程中才加入的，不应该被当前这轮 Tick 执行");
    }



    public TestContext TestContext { get; set; }
}
