using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Retryable;
using ThabeSoft.Primitives;

namespace ThabeSoft.EdgeGateway.UnitTests.Lifecycles;


[TestClass]
public class RetryableLifecycleStateMachineTests
{
    private RetryableLifecycleStateMachineResolver Resolver { get; set; } = null!;

    [TestInitialize]
    public void Setup()
    {
        var loggerFactory = LoggerFactory.Create(x => x.SetMinimumLevel(LogLevel.Trace));
        Resolver = new RetryableLifecycleStateMachineResolver(loggerFactory);
    }


    [TestMethod]
    public async Task StateTransitions()
    {
        var lifecycle = Resolver.Resolve(LifecycleBuilder.RetryableStateMachine());

        // 启动
        await lifecycle.TransitionToRunningAsync(EmptyMethod, TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        // 启动之后可以再次启动
        await lifecycle.TransitionToRunningAsync(EmptyMethod, TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        // 启动后停止
        await lifecycle.TransitionToStoppedAsync(EmptyMethod, TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State);

        // 停止之后可以再次停止
        await lifecycle.TransitionToStoppedAsync(EmptyMethod, TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Stopped, lifecycle.State);

        // 停止之后可以再次启动
        await lifecycle.TransitionToRunningAsync(EmptyMethod, TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        // 运行状态可以直接释放
        await Safe.DisposeSafelyAsync(lifecycle);
        Assert.AreEqual(LifecycleState.Terminated, lifecycle.State);

        // 释放之后不可启动
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await lifecycle.TransitionToRunningAsync(EmptyMethod, TestContext.CancellationToken));

        // 释放之后不可停止
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await lifecycle.TransitionToStoppedAsync(EmptyMethod, TestContext.CancellationToken));

        // 幂等释放
        await Safe.DisposeSafelyAsync(lifecycle);
        Assert.AreEqual(LifecycleState.Terminated, lifecycle.State);
    }

    [TestMethod]
    public async Task Start_Exception_StateToFaulted()
    {
        var lifecycle = Resolver.Resolve(LifecycleBuilder.RetryableStateMachine());

        await Assert.ThrowsAsync<Exception>(async () =>
            await lifecycle.TransitionToRunningAsync(ExceptionMethod, TestContext.CancellationToken));

        // 启动失败
        Assert.AreEqual(LifecycleState.Faulted, lifecycle.State);
    }

    [TestMethod]
    public async Task Stop_Exception_StateToFaulted()
    {
        var lifecycle = Resolver.Resolve(LifecycleBuilder.RetryableStateMachine());

        await lifecycle.TransitionToRunningAsync(EmptyMethod, TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, lifecycle.State);

        await Assert.ThrowsAsync<Exception>(async () =>
            await lifecycle.TransitionToStoppedAsync(ExceptionMethod, TestContext.CancellationToken));

        // 启动失败
        Assert.AreEqual(LifecycleState.Faulted, lifecycle.State);
    }

    private static ValueTask EmptyMethod(CancellationToken _) => default;
    private static ValueTask ExceptionMethod(CancellationToken _) => throw new Exception();
    private static ValueTask EmptyMethod() => default;


    public TestContext TestContext { get; set; }
}