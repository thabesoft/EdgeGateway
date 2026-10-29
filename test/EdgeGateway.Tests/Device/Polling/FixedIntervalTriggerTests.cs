using EdgeGateway.Tag.Polling;
using Microsoft.Extensions.Time.Testing;

namespace EdgeGateway.Device.Polling;


[TestClass]
public class FixedIntervalTriggerTests
{
    [TestMethod]
    public async Task Subscribe()
    {
        var fakeTime = new FakeTimeProvider();
        var interval = TimeSpan.FromSeconds(1);
        await using var trigger = new FixedIntervalTrigger(interval, fakeTime);


        var count = 0;
        var tickCompleted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);


        using var _ = trigger.Subscribe(async _ =>
        {
            await Task.Yield();
            Interlocked.Increment(ref count);
            Interlocked.Exchange(ref tickCompleted, null)?.TrySetResult();
        });

        await trigger.StartAsync(TestContext.CancellationToken);

        await AdvanceAndWaitAsync();
        Assert.AreEqual(1, Volatile.Read(ref count));

        await AdvanceAndWaitAsync();
        Assert.AreEqual(2, Volatile.Read(ref count));

        await AdvanceAndWaitAsync();
        Assert.AreEqual(3, Volatile.Read(ref count));

        await trigger.StopAsync(TestContext.CancellationToken);



        // 每次换新 TCS，等本次 tick 完成
        async Task AdvanceAndWaitAsync()
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            tickCompleted = tcs; // 需要让回调拿到当前 TCS
            fakeTime.Advance(interval);
            await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.CancellationToken);
        }
    }

    [TestMethod]
    public async Task Stop_ThenAdvance_DoesNotFire()
    {
        var fakeTime = new FakeTimeProvider();
        var interval = TimeSpan.FromSeconds(1);
        await using var trigger = new FixedIntervalTrigger(interval, fakeTime);

        var count = 0;
        using var sub = trigger.Subscribe(_ => { Interlocked.Increment(ref count); return ValueTask.CompletedTask; });

        await trigger.StartAsync(TestContext.CancellationToken);
        fakeTime.Advance(interval);
        await Task.Delay(50);
        var before = Volatile.Read(ref count);
        Assert.IsTrue(before >= 1);

        await trigger.StopAsync(TestContext.CancellationToken);

        fakeTime.Advance(interval);
        fakeTime.Advance(interval);
        await Task.Delay(50);
        Assert.AreEqual(before, Volatile.Read(ref count), "Stop 后不应再触发");
    }

    [TestMethod]
    public async Task Dispose_ThenSubscribe_Throws()
    {
        var trigger = new FixedIntervalTrigger(TimeSpan.FromSeconds(1), new FakeTimeProvider());
        await trigger.DisposeAsync();

        Assert.ThrowsExactly<ObjectDisposedException>(() =>
            trigger.Subscribe(_ => ValueTask.CompletedTask));
    }

    [TestMethod]
    public async Task DisposeSubscription_StopsReceiving()
    {
        var fakeTime = new FakeTimeProvider();
        var interval = TimeSpan.FromSeconds(1);
        await using var trigger = new FixedIntervalTrigger(interval, fakeTime);

        var count = 0;
        var sub = trigger.Subscribe(_ => { Interlocked.Increment(ref count); return ValueTask.CompletedTask; });

        await trigger.StartAsync(TestContext.CancellationToken);
        fakeTime.Advance(interval);
        await Task.Delay(50);
        Assert.AreEqual(1, Volatile.Read(ref count));

        sub.Dispose();  // 移除订阅

        fakeTime.Advance(interval);
        fakeTime.Advance(interval);
        await Task.Delay(50, TestContext.CancellationToken);
        Assert.AreEqual(1, Volatile.Read(ref count), "Dispose 订阅后不应再收到回调");
    }


    [TestMethod]
    public async Task MultipleSubscribers_AllInvoked()
    {
        var fakeTime = new FakeTimeProvider();
        var interval = TimeSpan.FromSeconds(1);
        await using var trigger = new FixedIntervalTrigger(interval, fakeTime);

        var a = 0; var b = 0;
        using var s1 = trigger.Subscribe(_ => { Interlocked.Increment(ref a); return ValueTask.CompletedTask; });
        using var s2 = trigger.Subscribe(_ => { Interlocked.Increment(ref b); return ValueTask.CompletedTask; });

        await trigger.StartAsync(TestContext.CancellationToken);
        fakeTime.Advance(interval);
        await Task.Delay(50);

        Assert.AreEqual(1, Volatile.Read(ref a));
        Assert.AreEqual(1, Volatile.Read(ref b));
    }

    public TestContext TestContext { get; set; }
}
