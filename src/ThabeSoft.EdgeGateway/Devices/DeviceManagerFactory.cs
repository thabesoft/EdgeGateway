using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Protocols;
using ThabeSoft.PubSub;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceManagerFactory(
        IEventPublisher eventPublisher,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory,
        DeviceFactory deviceFactory,
        ILoggerFactory loggerFactory
    )
{
    public DeviceManager Create(DeviceManagerCreationArgs args)
    {
        var logger = loggerFactory.CreateLogger<DeviceManager>();
        var collection = lifecycleKeyedCollectionFactory.Create<DeviceId, IDevice>(args.PortState);
        var batchExecutor = lifecycleBatchExecutorFactory.Create(args.Port.DeviceBatchExecutor);

        // 端口管理依赖
        var portManagerDependencies = new DeviceManagerDependencies
        {
            Port = args.Port,

            Collection = collection,
            BatchExecutor = batchExecutor,

            EventPublisher = eventPublisher,
            Logger = logger,

            DeviceCreator = device =>
            {
                var creationArgs = new DeviceCreationArgs
                {
                    Port = args.Port,
                    PortState = args.PortState,
                    ProtocolAccessor = args.ProtocolAccessorProvider(device.Protocol),
                    Device = device
                };

                return deviceFactory.Create(creationArgs);
            },
            OnDispose = async () =>
            {
                await Safe.DisposeSafelyAsync(batchExecutor, logger);
                await Safe.DisposeSafelyAsync(collection, logger);
            }
        };

        return new DeviceManager(portManagerDependencies);
    }
}


internal sealed class DeviceManagerCreationArgs
{
    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required ILifecycleStateable PortState { get; init => field = value.Required(); }

    // Device
    public required Func<DeviceDescriptor, Device> DevicCreator { get; init => field = value.Required(); }
    public required Func<ProtocolDescriptor, IProtocolAccessor> ProtocolAccessorProvider { get; init => field = value.Required(); }
}