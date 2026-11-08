using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class Device(
        DeviceDescriptor deviceDescriptor,
        SubscriptionFactory subscriptionFactory,
        IByteDuplex byteDuplex,
        ILifecycleState portLifecycleState,
        ILogger<Device> logger
    ) : IDevice
{
    private readonly LifecycleStateMachineA _stateMachine = Lifecycle.Normal(x => x.StartStop(30).Dispose(10), logger: logger);
    private readonly ConcurrentDictionary<SubscriptionId, ISubscription> _subscriptions = [];


    public DeviceId Id => deviceDescriptor.Id;
    public ProtocolType ProtocolType => deviceDescriptor.Protocol.Type;
    public IReadOnlyDictionary<SubscriptionId, ISubscription> Subscriptions => _subscriptions;
    public LifecycleState State => _stateMachine.State;


    public ISubscription<T> AddSubscription<T>(SubscriptionDescriptor<T> subscriptionDescriptor) where T : notnull
    {
        return _stateMachine.Execute(() =>
        {
            var subscription = subscriptionFactory.Create(subscriptionDescriptor, deviceDescriptor.Protocol, byteDuplex, this);
            _subscriptions[subscriptionDescriptor.Id] = subscription;
            return subscription;
        });
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);


    private async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (portLifecycleState.State is not (LifecycleState.Starting or LifecycleState.Running))
            throw new InvalidOperationException("设备启动失败, 端口未启动");

        List<Exception> errors = [];
        foreach (var subscription in Subscriptions.Values)
        {
            try
            {
                await subscription.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                logger.LogError(ex, "订阅 {SubscriptionId} 启动失败", subscription.Id);
            }
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"设备 {Id} 启动时 {errors.Count} 个订阅启动失败", errors);
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        foreach (var subscription in Subscriptions.Values)
        {
            try
            {
                await subscription.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                logger.LogError(ex, "订阅 {SubscriptionId} 停止失败", subscription.Id);
            }
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"设备 {Id} 停止时 {errors.Count} 个订阅停止失败", errors);
    }
    private async ValueTask OnDisposeAsync()
    {
        foreach (var subscription in Subscriptions.Values)
        {
            try { await subscription.DisposeAsync(); }
            catch (Exception ex)
            {
                logger.LogError(ex, "订阅 {SubscriptionId} 释放失败", subscription.Id);
            }
        }
    }
}