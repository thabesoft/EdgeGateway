using Microsoft.Extensions.Logging;
using NModbus;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports.Modbus.Ip;
using ThabeSoft.EdgeGateway.Ports.Modbus.Serial;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Ports.Modbus;


internal sealed class ModbusSimulationPortResolver(
        IModbusFactory modbusFactory,
        SimulationDeviceFactory deviceFactorySelector,
        ILoggerFactory loggerFactory
    ) : ISimulationPortResolver
{
    public bool CanCreate(SimulatorPortDescriptor descriptor)
    {
        return descriptor.Protocol.Type == ProtocolType.Modbus;
    }

    public ISimulationPort Resolve(SimulatorPortDescriptor descriptor)
    {
        if (descriptor.Protocol is not ModbusProtocolDescriptor modbus)
            throw new InvalidOperationException("不是Modbus协议");


        var baseArgs = new SimulationPortBaseArgs()
        {
            Transport = descriptor.Transport,
            PortStateMachine = 
        };


        if (descriptor.Transport is SerialTransportDescriptor serial)
        {
            var args = new ModbusSerialSimulationPortArgs()
            {

            };

            if (modbus.FrameFormat == ModbusFrameFormat.Rtu)
                return ModbusSerialSimulationPort.Rtu(serial, modbusFactory, deviceFactorySelector, loggerFactory.CreateLogger<ModbusSerialSimulationPort>());

            if (modbus.FrameFormat == ModbusFrameFormat.Ascii)
                return ModbusSerialSimulationPort.Ascii(serial, modbusFactory, deviceFactorySelector, loggerFactory.CreateLogger<ModbusSerialSimulationPort>());
        }

        if (descriptor.Transport is IpTransportDescriptor ip)
        {
            if (ip.ProtocolType == IpProtocolType.Tcp)
                return new ModbusTcpSimulationPort(ip, modbusFactory, deviceFactorySelector, loggerFactory.CreateLogger<ModbusTcpSimulationPort>());

            if (ip.ProtocolType == IpProtocolType.Udp)
                return new ModbusUdpSimulationPort(ip, modbusFactory, deviceFactorySelector, loggerFactory.CreateLogger<ModbusUdpSimulationPort>());
        }


        throw new InvalidOperationException($"不支持的描述符: {descriptor}");
    }
}