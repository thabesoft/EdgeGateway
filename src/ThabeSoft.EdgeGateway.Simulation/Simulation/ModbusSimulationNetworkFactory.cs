using Microsoft.Extensions.Logging;
using NModbus;
using ThabeSoft.EdgeGateway.Ethernet;
using ThabeSoft.EdgeGateway.Simulation.Networks;

namespace ThabeSoft.EdgeGateway.Simulation;


internal sealed class ModbusSimulationNetworkFactory(
        IModbusFactory modbusFactory,
        ModbusSimulatedSlaveFactory slaveSimulatorFactory,
        ILoggerFactory loggerFactory
    )
{
    public ModbusSimulationNetwork Create(ModbusSimulatorNetworkDescriptor descriptor)
    {
        if (descriptor.Protocol is not ModbusProtocolDescriptor modbus)
            throw new InvalidOperationException("不是Modbus协议");

        if (descriptor.Transport is SerialTransportDescriptor serial)
        {
            if (modbus.FrameFormat == ModbusFrameFormat.Rtu)
                return ModbusSerialSimulationNetwork.Rtu(serial, modbusFactory, slaveSimulatorFactory, loggerFactory.CreateLogger<ModbusSerialSimulationNetwork>());

            if (modbus.FrameFormat == ModbusFrameFormat.Ascii)
                return ModbusSerialSimulationNetwork.Ascii(serial, modbusFactory, slaveSimulatorFactory, loggerFactory.CreateLogger<ModbusSerialSimulationNetwork>());
        }

        if (descriptor.Transport is IpTransportDescriptor ip)
        {
            if (ip.TransportType == IpTransportType.Tcp)
                return new ModbusTcpSimulationNetwork(ip, modbusFactory, slaveSimulatorFactory, loggerFactory.CreateLogger<ModbusTcpSimulationNetwork>());

            if (ip.TransportType == IpTransportType.Udp)
                return new ModbusUdpSimulationNetwork(ip, modbusFactory, slaveSimulatorFactory, loggerFactory.CreateLogger<ModbusUdpSimulationNetwork>());
        }


        throw new InvalidOperationException($"不支持的描述符: {descriptor}");
    }
}