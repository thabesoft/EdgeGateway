using Microsoft.Extensions.Logging;
using NModbus;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Devices.Modbus;


internal sealed class ModbusSimulationDeviceResolver(
        IModbusFactory modbusFactory,
        TriggerManager triggerManager,
        ILoggerFactory loggerFactory,
        ICodecFactory codecFactory,
        ILifecycleStateMachineFactory lifecycleStateMachineFactory
    ) : ISimulationDeviceResolver
{
    public bool CanResolve(SimulationDeviceCreationContext context)
    {
        return context.Device.Id is ModbusSimulationDeviceId;
    }

    public ISimulationDevice Resolve(SimulationDeviceCreationContext context)
    {
        if (context.Device.Id is not ModbusSimulationDeviceId deviceId)
            throw new InvalidOperationException("不支持的从站Id");

        var args = new ModbusSimulationDeviceArgs()
        {
            Base = new()
            {
                Device = context.Device,
                DeviceStateMachine = context.DeviceStateMachine,
                LifecycleStateMachineFactory = lifecycleStateMachineFactory,
                Logger = loggerFactory.CreateLogger<ModbusSimulationDevice>(),
                TaskManager = new Utilities.TaskManager(),
                TriggerManager = triggerManager,
            },
            ModbusFactory = modbusFactory,
            UnitId = deviceId.Value,
            CodecFactory = codecFactory
        };

        return new ModbusSimulationDevice(args);
    }
}
