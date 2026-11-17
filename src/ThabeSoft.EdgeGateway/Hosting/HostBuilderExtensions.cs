using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.DependencyInjection;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Hosting;


public static class HostBuilderExtensions
{
    extension(IHostBuilder hostBuilder)
    {
        public IHostBuilder UseEdgeGateway(Action<GatewayHostBuilder> buildAction)
        {
            var gatewayBuilder = new GatewayHostBuilder();
            buildAction.Invoke(gatewayBuilder);


            return hostBuilder.ConfigureServices((_, descriptors) =>
            {
                descriptors.AddEdgeGatewayEvents();
                descriptors.AddEdgeGatewayTransport();
                descriptors.AddEdgeGatewayProtocol();
                descriptors.AddEdgeGatewayReadings();

                descriptors.AddEdgeGatewaySubscriptions();
                descriptors.AddEdgeGatewayDevices();
                descriptors.AddEdgeGatewayPorts();

                descriptors.AddEdgeGateway(gatewayBuilder.Build());
                descriptors.AddEdgeGatewayHost();
            });
        }
    }
}


public sealed class GatewayHostBuilder
{
    // Gateway
    internal LifecycleStateMachineDescriptor? GatewayStateMachine { get; private set => field = value.Required(); }

    // Port
    internal LifecycleStateMachineDescriptor? PortStateMachine { get; private set => field = value.Required(); }
    internal LifecycleStateMachineDescriptor? TransportStateMachine { get; private set => field = value.Required(); }
    internal LifecycleBatchExecutorDescriptor PortBatchExecutor { get; private set => field = value.Required(); } = new LifecycleBatchExecutorDescriptor();


    // Device
    internal LifecycleStateMachineDescriptor? DeviceStateMachine { get; private set => field = value.Required(); }
    internal LifecycleBatchExecutorDescriptor DeviceBatchExecutor { get; private set => field = value.Required(); } = new LifecycleBatchExecutorDescriptor();


    // Subscription
    internal LifecycleStateMachineDescriptor? SubscriptionStateMachine { get; private set => field = value.Required(); }
    internal LifecycleBatchExecutorDescriptor SubscriptionBatchExecutor { get; private set => field = value.Required(); } = new LifecycleBatchExecutorDescriptor();


    internal GatewayCreationArgs Build()
    {
        return new()
        {
            GatewayStateMachine = GatewayStateMachine!,

            PortStateMachine = PortStateMachine!,
            TransportStateMachine = TransportStateMachine!,
            PortBatchExecutor = PortBatchExecutor,

            DeviceStateMachine = DeviceStateMachine!,
            DeviceBatchExecutor = DeviceBatchExecutor,

            SubscriptionStateMachine = SubscriptionStateMachine!,
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