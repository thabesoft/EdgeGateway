namespace ThabeSoft.EdgeGateway.Lifecycles;

[TestClass]
public sealed class ConcurrentLifecycleTests
{
    [TestMethod]
    public async Task Concurrent_Start_Should_Execute_OnStartAsync_Only_Once()
    {
        var lifecycle = new CounterLifecycle(TestContext);

        var tasks = Enumerable.Range(1, 100).Select(_ => lifecycle.StartAsync(TestContext.CancellationToken).AsTask());
        await Task.WhenAll(tasks);


        Assert.AreEqual(1, lifecycle.StartCallCount, "并发启动 100 次，OnStartAsync 只能被真正执行 1 次！");
        Assert.AreEqual(0, lifecycle.StopCallCount, "没有被调用 Stop，OnStopAsync 不应执行！");
        Assert.AreEqual(LifecycleState.Running, lifecycle.State, "最终状态必须是 Running！");
    }

    [TestMethod]
    public async Task Start_Then_Immediate_Stop_Should_Queue_And_Not_Tear_State()
    {
        // 1. 准备
        var lifecycle = new CounterLifecycle(TestContext);

        // 2. 同时发起“启动”和“停止”
        // 因为 SemaphoreSlim 的存在，它们必然排队执行（先启动，后停止）
        var startTask = lifecycle.StartAsync(TestContext.CancellationToken).AsTask();
        var stopTask = lifecycle.StopAsync(TestContext.CancellationToken).AsTask();

        await Task.WhenAll(startTask, stopTask);

        // 3. 验证
        Assert.AreEqual(1, lifecycle.StartCallCount);
        Assert.AreEqual(1, lifecycle.StopCallCount);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State, "最终状态必须是 Stopped！");
    }

    [TestMethod]
    public async Task Concurrent_Start_Stop_Extreme()
    {
        var lifecycle = new CounterLifecycle(TestContext);
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
        var lifecycle = new CounterLifecycle(TestContext);

        // 并发发起 100 个启动任务，并瞬间全部取消！
        var cts = new CancellationTokenSource();
        var tasks = Enumerable.Range(0, 100).Select(_ => lifecycle.StartAsync(cts.Token).AsTask()).ToList();

        cts.Cancel(); // 瞬间取消

        try { await Task.WhenAll(tasks); }
        catch (OperationCanceledException) { /* 忽略预期取消 */ }

        // 核心验证：如果锁被死锁了，这行 StopAsync 会永远卡在这里，导致测试超时失败
        await lifecycle.StopAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State, "取消风暴后，锁必须被释放，状态机依然存活");
    }

    [TestMethod]
    public async Task Dispose_Should_Cleanup_Resources_Even_If_Start_Failed()
    {
        var lifecycle = new StartExceptionLifecycle(); // 这个类的 OnStartAsync 会抛异常，但会先分配一个模拟资源

        try { await lifecycle.StartAsync(TestContext.CancellationToken); } catch { }

        await lifecycle.DisposeAsync();

        Assert.IsTrue(lifecycle.IsResourceReleased, "启动失败后执行 Dispose，底层资源必须被释放！");
        Assert.AreEqual(LifecycleState.Disposed, lifecycle.State);
    }

    [TestMethod]
    public async Task Invalid_State_Transitions_Should_Be_Ignored()
    {
        var lifecycle = new CounterLifecycle(TestContext);

        // 1. 初始状态直接 Stop（应该被幂等拦截，不改变状态）
        await lifecycle.StopAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Created, lifecycle.State, "在 Created 状态调 Stop，状态不应改变");

        // 2. 启动后连续两次 Stop
        await lifecycle.StartAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        await lifecycle.StopAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State);

        // 3. 第二次 Stop（应该被忽略，不能报错，也不能回到 Running）
        await lifecycle.StopAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State, "连续 Stop 必须保持 Stopped 状态");
    }



    public TestContext TestContext { get; set; }
}


file sealed class CounterLifecycle(TestContext testContext) : ConcurrentLifecycleBase()
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
file sealed class StartExceptionLifecycle : ConcurrentLifecycleBase
{
    public bool IsResourceReleased { get; private set; }
    protected override async ValueTask OnStartAsync(CancellationToken ct)
    {
        await Task.Yield();
        throw new Exception("模拟硬件握手失败");
    }
    protected override ValueTask OnStopAsync(CancellationToken ct) =>
        ValueTask.CompletedTask;
    protected override ValueTask OnDisposeAsync()
    {
        IsResourceReleased = true;
        return ValueTask.CompletedTask;
    }
}