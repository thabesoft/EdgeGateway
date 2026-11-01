namespace ThabeSoft.EdgeGateway.Lifecycles;


[TestClass]
public class ResilientLifecycleTests
{
    [TestMethod]
    public async Task Start_While_Running_Should_Execute_OnStartAsync_Again()
    {
        var lifecycle = new ResilientCounterLifecycle(TestContext);

        // 1. 首次启动
        await lifecycle.StartAsync(TestContext.CancellationToken);
        Assert.AreEqual(1, lifecycle.StartCallCount);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        // 2. 在运行中再次启动（模拟补启逻辑）
        await lifecycle.StartAsync(TestContext.CancellationToken);

        // 3. 验证
        Assert.AreEqual(2, lifecycle.StartCallCount, "在 Running 状态下再次启动，OnStartAsync 必须被再次执行！");
        Assert.AreEqual(LifecycleState.Running, lifecycle.State, "补启完成后，状态依然保持 Running！");
    }

    [TestMethod]
    public async Task Concurrent_Start_While_Running_Should_Execute_Strictly_Serial()
    {
        var lifecycle = new ResilientCounterLifecycle(TestContext);
        await lifecycle.StartAsync(TestContext.CancellationToken); // 先让它进入 Running 状态

        // 并发发起 50 次补启请求
        var tasks = Enumerable.Range(1, 50).Select(_ => lifecycle.StartAsync(TestContext.CancellationToken).AsTask());
        await Task.WhenAll(tasks);

        // 核心验证：加上首次启动的 1 次，总共应该执行 51 次
        Assert.AreEqual(51, lifecycle.StartCallCount, "50 次并发补启必须全部执行！");
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);
    }

    [TestMethod]
    public async Task Start_Then_Immediate_Stop_Should_Queue_And_Not_Tear_State()
    {
        var lifecycle = new ResilientCounterLifecycle(TestContext);

        // 同时发起“启动”和“停止”
        var startTask = lifecycle.StartAsync(TestContext.CancellationToken).AsTask();
        var stopTask = lifecycle.StopAsync(TestContext.CancellationToken).AsTask();

        await Task.WhenAll(startTask, stopTask);

        // 验证：因为抢锁，Start 先执行完，Stop 再执行
        Assert.AreEqual(1, lifecycle.StartCallCount);
        Assert.AreEqual(1, lifecycle.StopCallCount);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State, "最终状态必须是 Stopped！");
    }

    [TestMethod]
    public async Task Concurrent_Start_Stop_Extreme()
    {
        var lifecycle = new ResilientCounterLifecycle(TestContext);
        var random = new Random();

        // 500 个并发任务，每个任务随机执行 10 次 Start / Stop / Dispose 的混合攻击
        var attacks = Enumerable.Range(0, 500).Select(async _ =>
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    int action = random.Next(0, 3);
                    if (action == 0) await lifecycle.StartAsync(TestContext.CancellationToken);
                    if (action == 1) await lifecycle.StopAsync(TestContext.CancellationToken);
                    if (action == 2) await lifecycle.DisposeAsync();
                }
                catch (ObjectDisposedException) { /* 预期的异常 */ }
                catch (OperationCanceledException) { /* 预期的取消 */ }
            }
        });

        await Task.WhenAll(attacks);

        // 最终无论如何，状态必须是一个绝对合法的终态
        Assert.IsTrue(lifecycle.State == LifecycleState.Disposed || lifecycle.State == LifecycleState.Running || lifecycle.State == LifecycleState.Stopped);
    }

    [TestMethod]
    public async Task Cancellation_Storm_Should_Not_Deadlock()
    {
        var lifecycle = new ResilientCounterLifecycle(TestContext);

        var cts = new CancellationTokenSource();
        var tasks = Enumerable.Range(0, 100).Select(_ => lifecycle.StartAsync(cts.Token).AsTask()).ToList();

        cts.Cancel(); // 瞬间取消

        try { await Task.WhenAll(tasks); }
        catch (OperationCanceledException) { }

        // 核心验证：如果锁被死锁了，这行 StopAsync 会永远卡在这里
        await lifecycle.StopAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State, "取消风暴后，锁必须被释放，状态机依然存活");
    }

    [TestMethod]
    public async Task Dispose_Should_Cleanup_Resources_Even_If_Start_Failed()
    {
        var lifecycle = new ResilientStartExceptionLifecycle();

        try { await lifecycle.StartAsync(TestContext.CancellationToken); } catch { }

        await lifecycle.DisposeAsync();

        Assert.IsTrue(lifecycle.IsResourceReleased, "启动失败后执行 Dispose，底层资源必须被释放！");
        Assert.AreEqual(LifecycleState.Disposed, lifecycle.State);
    }

    [TestMethod]
    public async Task Invalid_State_Transitions_Should_Be_Ignored()
    {
        var lifecycle = new ResilientCounterLifecycle(TestContext);

        // 1. 初始状态直接 Stop
        await lifecycle.StopAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Created, lifecycle.State, "在 Created 状态调 Stop，状态不应改变");

        // 2. 启动后连续两次 Stop
        await lifecycle.StartAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        await lifecycle.StopAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State);

        await lifecycle.StopAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State, "连续 Stop 必须保持 Stopped 状态");
    }

    [TestMethod]
    public async Task Start_Failure_Should_Rollback_State_Correctly()
    {
        var lifecycle = new ResilientStartExceptionLifecycle();

        // 启动抛异常
        await Assert.ThrowsAsync<Exception>(async () =>
            await lifecycle.StartAsync(TestContext.CancellationToken));

        // 状态回滚为 Stopped
        Assert.AreEqual(LifecycleState.Created, lifecycle.State, "启动失败后，状态必须回滚！");
    }


    public TestContext TestContext { get; set; }
}


file sealed class ResilientCounterLifecycle(TestContext testContext) : ResilientLifecycleBase()
{
    public int StartCallCount { get; private set; }
    public int StopCallCount { get; private set; }

    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        StartCallCount++;
        testContext.WriteLine("OnStartAsync called.");
        await Task.Yield();
    }

    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        StopCallCount++;
        testContext.WriteLine("OnStopAsync called.");
        await Task.Yield();
    }
}

file sealed class ResilientStartExceptionLifecycle : ResilientLifecycleBase
{
    public bool IsResourceReleased { get; private set; }

    protected override async ValueTask OnStartAsync(CancellationToken ct)
    {
        await Task.Yield();
        throw new Exception("模拟硬件握手失败");
    }

    protected override ValueTask OnStopAsync(CancellationToken ct) => ValueTask.CompletedTask;

    protected override ValueTask OnDisposeAsync()
    {
        IsResourceReleased = true;
        return ValueTask.CompletedTask;
    }
}