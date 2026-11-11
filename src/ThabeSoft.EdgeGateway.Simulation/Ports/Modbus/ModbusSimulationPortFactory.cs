using Microsoft.Extensions.Logging;
using NModbus;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Devices.Modbus;
using ThabeSoft.EdgeGateway.Networks.Modbus;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Slaves.Modbus;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports.Modbus;


internal sealed class ModbusSimulationPortFactory(
        IModbusFactory modbusFactory,
        SimulationDeviceFactorySelector deviceFactorySelector,
        ILoggerFactory loggerFactory
    ) : ISimulationPortFactory
{
    public bool CanCreate(SimulatorPortDescriptor descriptor)
    {
        return descriptor.Protocol.Type == ProtocolType.Modbus;
    }

    public ISimulationPort Create(SimulatorPortDescriptor descriptor)
    {
        if (descriptor.Protocol is not ModbusProtocolDescriptor modbus)
            throw new InvalidOperationException("不是Modbus协议");

        if (descriptor.Transport is SerialTransportDescriptor serial)
        {
            if (modbus.FrameFormat == ModbusFrameFormat.Rtu)
                return ModbusSerialSimulationPort.Rtu(serial, modbusFactory, deviceFactorySelector, loggerFactory.CreateLogger<ModbusSerialSimulationPort>());

            if (modbus.FrameFormat == ModbusFrameFormat.Ascii)
                return ModbusSerialSimulationPort.Ascii(serial, modbusFactory, deviceFactorySelector, loggerFactory.CreateLogger<ModbusSerialSimulationPort>());
        }

        if (descriptor.Transport is IpTransportDescriptor ip)
        {
            if (ip.TransportType == IpTransportType.Tcp)
                return new ModbusTcpSimulationPort(ip, modbusFactory, deviceFactorySelector, loggerFactory.CreateLogger<ModbusTcpSimulationPort>());

            if (ip.TransportType == IpTransportType.Udp)
                return new ModbusUdpSimulationPort(ip, modbusFactory, deviceFactorySelector, loggerFactory.CreateLogger<ModbusUdpSimulationPort>());
        }


        throw new InvalidOperationException($"不支持的描述符: {descriptor}");
    }
}