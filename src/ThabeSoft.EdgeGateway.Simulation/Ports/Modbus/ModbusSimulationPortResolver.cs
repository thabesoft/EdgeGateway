using Microsoft.Extensions.Logging;
using NModbus;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.Transports.Ethernet;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Modbus;
using ThabeSoft.EdgeGateway.Ports.Modbus.Ip;
using ThabeSoft.EdgeGateway.Ports.Modbus.Serial;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Transports.Serial;

namespace ThabeSoft.EdgeGateway.Ports.Modbus;


internal sealed class ModbusSimulationPortResolver(
        IModbusFactory modbusFactory,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
        ILifecycleStateMachineFactory lifecycleStateMachineFactory,
        SimulationDeviceFactory simulationDeviceFactory,
        ILoggerFactory loggerFactory
    ) : ISimulationPortResolver
{
    public bool CanCreate(SimulatorPortCreationContext context)
    {
        return context.Port.Protocol.Type == ProtocolType.Modbus;
    }

    public ISimulationPort Resolve(SimulatorPortCreationContext context)
    {
        if (context.Port.Protocol is not ModbusProtocolDescriptor modbus)
            throw new InvalidOperationException("不是Modbus协议");


        var baseArgs = new SimulationPortBaseArgs()
        {
            Transport = context.Port.Transport,
            PortStateMachine = context.PortStateMachine,

            DeviceBatchExecutor = context.DeviceBatchExecutor,
            DeviceFactory = simulationDeviceFactory,
            DeviceStateMachine = context.DeviceStateMachine,

            LifecycleBatchExecutorFactory = lifecycleBatchExecutorFactory,
            LifecycleKeyedCollectionFactory = lifecycleKeyedCollectionFactory,
            LifecycleStateMachineFactory = lifecycleStateMachineFactory,
            Logger = loggerFactory.CreateLogger<SimulationPortBase>()
        };


        if (context.Port.Transport is SerialTransportDescriptor serial)
        {
            var args = new ModbusSerialSimulationPortArgs()
            {
                Base = baseArgs,
                ModbusFactory = modbusFactory,
                Transport = serial
            };

            if (modbus.FrameFormat == ModbusFrameFormat.Rtu)
                return ModbusSerialSimulationPort.Rtu(args);

            if (modbus.FrameFormat == ModbusFrameFormat.Ascii)
                return ModbusSerialSimulationPort.Ascii(args);
        }

        if (context.Port.Transport is IpTransportDescriptor ip)
        {
            var args = new ModbusIpSimulationPortArgs()
            {
                Base = baseArgs,
                ModbusFactory = modbusFactory,
                Transport = ip
            };

            if (ip.ProtocolType == IpProtocolType.Tcp)
                return new ModbusTcpSimulationPort(args);

            if (ip.ProtocolType == IpProtocolType.Udp)
                return new ModbusUdpSimulationPort(args);
        }


        throw new InvalidOperationException($"不支持的描述符: {context}");
    }
}