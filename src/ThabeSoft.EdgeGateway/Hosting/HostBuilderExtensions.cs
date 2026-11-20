using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.DependencyInjection;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Hosting;


public static class HostBuilderExtensions
{
    extension(IHostBuilder hostBuilder)
    {
        /// <summary>
        /// Event, Transport, Protocol, Readings, Subscriptions, Devices, Ports, Host
        /// </summary>
        public IHostBuilder UseEdgeGateway(Action<GatewayHostBuilder>? buildAction = null)
        {
            var gatewayBuilder = new GatewayHostBuilder();
            buildAction?.Invoke(gatewayBuilder);


            return hostBuilder.ConfigureServices((_, descriptors) =>
            {
                descriptors.AddEdgeGatewayReadings();
                descriptors.AddEdgeGatewaySubscriptions();
                descriptors.AddEdgeGatewayDevices();
                descriptors.AddEdgeGatewayPorts();
                descriptors.AddEdgeGateway(gatewayBuilder.Build());

                descriptors.AddEdgeGatewayConfig();
                descriptors.AddEdgeGatewayHost();
            });
        }
    }
}


public sealed class GatewayHostBuilder
{
    // Gateway
    internal LifecycleStateMachineDescriptor GatewayStateMachine { get; private set => field = value.Required(); } = LifecycleBuilder.RetryableStateMachine();

    // Port
    internal LifecycleStateMachineDescriptor PortStateMachine { get; private set => field = value.Required(); } = LifecycleBuilder.RetryableStateMachine();
    internal LifecycleStateMachineDescriptor TransportStateMachine { get; private set => field = value.Required(); } = LifecycleBuilder.StrictStateMachine();
    internal LifecycleBatchExecutorDescriptor PortBatchExecutor { get; private set => field = value.Required(); } = LifecycleBuilder.BatchExecutor();


    // Device
    internal LifecycleStateMachineDescriptor DeviceStateMachine { get; private set => field = value.Required(); } = LifecycleBuilder.RetryableStateMachine();
    internal LifecycleBatchExecutorDescriptor DeviceBatchExecutor { get; private set => field = value.Required(); } = LifecycleBuilder.BatchExecutor();


    // Subscription
    internal LifecycleStateMachineDescriptor SubscriptionStateMachine { get; private set => field = value.Required(); } = LifecycleBuilder.StrictStateMachine();
    internal LifecycleBatchExecutorDescriptor SubscriptionBatchExecutor { get; private set => field = value.Required(); } = LifecycleBuilder.BatchExecutor();


    internal GatewayCreationArgs Build()
    {
        return new()
        {
            GatewayStateMachine = GatewayStateMachine,

            PortStateMachine = PortStateMachine,
            TransportStateMachine = TransportStateMachine,
            PortBatchExecutor = PortBatchExecutor,

            DeviceStateMachine = DeviceStateMachine,
            DeviceBatchExecutor = DeviceBatchExecutor,

            SubscriptionStateMachine = SubscriptionStateMachine,
            SubscriptionBatchExecutor = SubscriptionBatchExecutor
        };
    }



    public GatewayHostBuilder WithGatewayStateMachine(LifecycleStateMachineDescriptor stateMachine)
    {
        GatewayStateMachine = stateMachine;
        return this;
    }


    public GatewayHostBuilder WithPortStateMachine(LifecycleStateMachineDescriptor stateMachine)
    {
        PortStateMachine = stateMachine;
        return this;

    }
    public GatewayHostBuilder WithTransportStateMachine(LifecycleStateMachineDescriptor stateMachine)
    {
        TransportStateMachine = stateMachine;
        return this;
    }
    public GatewayHostBuilder WithPortBatchExecutor(LifecycleBatchExecutorDescriptor batchExecutor)
    {
        PortBatchExecutor = batchExecutor;
        return this;
    }


    public GatewayHostBuilder WithDeviceStateMachine(LifecycleStateMachineDescriptor stateMachine)
    {
        DeviceStateMachine = stateMachine;
        return this;
    }
    public GatewayHostBuilder WithDeviceBatchExecutor(LifecycleBatchExecutorDescriptor batchExecutor)
    {
        DeviceBatchExecutor = batchExecutor;
        return this;
    }


    public GatewayHostBuilder WithSubscriptionStateMachine(LifecycleStateMachineDescriptor stateMachine)
    {
        SubscriptionStateMachine = stateMachine;
        return this;
    }
    public GatewayHostBuilder WithSubscriptionBatchExecutor(LifecycleBatchExecutorDescriptor batchExecutor)
    {
        SubscriptionBatchExecutor = batchExecutor;
        return this;
    }
}