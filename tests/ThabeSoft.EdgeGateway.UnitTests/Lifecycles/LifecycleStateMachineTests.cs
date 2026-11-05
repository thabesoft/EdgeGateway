namespace ThabeSoft.EdgeGateway.Lifecycles;

[TestClass]
public sealed class LifecycleStateMachineTests
{
    [TestMethod]
    public async Task StateTransitions()
    {
        var lifecycle = new Normal(TestContext);

        // 启动
        await lifecycle.StartAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        // 启动之后不能再次启动
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await lifecycle.StartAsync(TestContext.CancellationToken));

        // 启动后停止
        await lifecycle.StopAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State);

        // 停止之后不能再次停止
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await lifecycle.StopAsync(TestContext.CancellationToken));

        // 停止之后可以再次启动
        await lifecycle.StartAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        // 运行状态可以直接释放
        await lifecycle.DisposeAsync();
        Assert.AreEqual(LifecycleState.Disposed, lifecycle.State);

        // 释放之后不可启动
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await lifecycle.StartAsync(TestContext.CancellationToken));

        // 释放之后不可停止
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await lifecycle.StopAsync(TestContext.CancellationToken));

        // 幂等释放
        await lifecycle.DisposeAsync();
        Assert.AreEqual(LifecycleState.Disposed, lifecycle.State);
    }

    [TestMethod]
    public async Task Start_Exception_StateToFaulted()
    {
        var lifecycle = new StartException();

        await Assert.ThrowsAsync<Exception>(async () =>
            await lifecycle.StartAsync(TestContext.CancellationToken));

        // 启动失败
        Assert.AreEqual(LifecycleState.Faulted, lifecycle.State);
    }

    [TestMethod]
    public async Task Stop_Exception_StateToFaulted()
    {
        var lifecycle = new StopException();

        await lifecycle.StartAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        await Assert.ThrowsAsync<Exception>(async () =>
            await lifecycle.StopAsync(TestContext.CancellationToken));

        // 启动失败
        Assert.AreEqual(LifecycleState.Faulted, lifecycle.State);
    }


    public TestContext TestContext { get; set; }
}


file sealed class Normal(TestContext testContext) : LifecycleStateMachine()
{
    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        testContext.WriteLine("已启动");
        await Task.Yield();
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        testContext.WriteLine("已停止");
        await Task.Yield();
    }
    protected override async ValueTask OnDisposeAsync()
    {
        testContext.WriteLine("已释放");
        await Task.Yield();
    }
}
file sealed class StartException : LifecycleStateMachine
{
    protected override async ValueTask OnStartAsync(CancellationToken ct)
    {
        await Task.Yield();
        throw new Exception();
    }
    protected override async ValueTask OnStopAsync(CancellationToken ct)
    {
        await Task.Yield();
    }
    protected override async ValueTask OnDisposeAsync()
    {
        await Task.Yield();
    }
}
file sealed class StopException : LifecycleStateMachine
{
    protected override async ValueTask OnStartAsync(CancellationToken ct)
    {
        await Task.Yield();
    }
    protected override async ValueTask OnStopAsync(CancellationToken ct)
    {
        await Task.Yield();
        throw new Exception();
    }
    protected override async ValueTask OnDisposeAsync()
    {
        await Task.Yield();
    }
}