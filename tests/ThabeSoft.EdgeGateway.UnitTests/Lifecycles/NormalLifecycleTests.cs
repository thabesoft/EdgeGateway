namespace ThabeSoft.EdgeGateway.Lifecycles;


[TestClass]
public class NormalLifecycleTests
{
    [TestMethod]
    public async Task StateTransitions()
    {
        var lifecycle = Lifecycle.StrictStateMachine(x => x.Start());

        // 启动
        await lifecycle.StartAsync(EmptyMethod, TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        // 启动之后可以再次启动
        await lifecycle.StartAsync(EmptyMethod, TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        // 启动后停止
        await lifecycle.StopAsync(EmptyMethod, TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State);

        // 停止之后可以再次停止
        await lifecycle.StopAsync(EmptyMethod, TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State);

        // 停止之后可以再次启动
        await lifecycle.StartAsync(EmptyMethod, TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        // 运行状态可以直接释放
        await lifecycle.DisposeAsync(EmptyMethod);
        Assert.AreEqual(LifecycleState.Disposed, lifecycle.State);

        // 释放之后不可启动
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await lifecycle.StartAsync(EmptyMethod, TestContext.CancellationToken));

        // 释放之后不可停止
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await lifecycle.StopAsync(EmptyMethod, TestContext.CancellationToken));

        // 幂等释放
        await lifecycle.DisposeAsync(EmptyMethod);
        Assert.AreEqual(LifecycleState.Disposed, lifecycle.State);
    }

    [TestMethod]
    public async Task Start_Exception_StateToFaulted()
    {
        var lifecycle = Lifecycle.StrictStateMachine();

        await Assert.ThrowsAsync<Exception>(async () =>
            await lifecycle.StartAsync(ExceptionMethod, TestContext.CancellationToken));

        // 启动失败
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);
    }

    [TestMethod]
    public async Task Stop_Exception_StateToFaulted()
    {
        var lifecycle = Lifecycle.StrictStateMachine();

        await lifecycle.StartAsync(EmptyMethod, TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        await Assert.ThrowsAsync<Exception>(async () =>
            await lifecycle.StopAsync(ExceptionMethod, TestContext.CancellationToken));

        // 启动失败
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State);
    }

    private static ValueTask EmptyMethod(CancellationToken _) => ValueTask.CompletedTask;
    private static ValueTask ExceptionMethod(CancellationToken _) => throw new Exception();
    private static ValueTask EmptyMethod() => ValueTask.CompletedTask;


    public TestContext TestContext { get; set; }
}