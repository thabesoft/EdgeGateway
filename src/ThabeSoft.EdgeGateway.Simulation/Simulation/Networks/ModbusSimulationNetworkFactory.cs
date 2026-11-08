using NModbus;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Simulation.Networks;


internal sealed class ModbusSimulationNetworkFactory(IModbusFactory modbusFactory, ModbusSimulatedSlaveFactory slaveSimulatorFactory)
{
    public ModbusSimulationNetwork Create(ModbusSimulatorNetworkDescriptor descriptor)
    {
        if (descriptor.Protocol is not ModbusProtocolDescriptor modbus)
            throw new InvalidOperationException("不是Modbus协议");

        if (descriptor.Transport is SerialTransportDescriptor serial)
        {
            if (modbus.FrameFormat == ModbusFrameFormat.Rtu)
                return ModbusSerialSimulationNetwork.Rtu(serial, modbusFactory, slaveSimulatorFactory);

            if (modbus.FrameFormat == ModbusFrameFormat.Ascii)
                return ModbusSerialSimulationNetwork.Ascii(serial, modbusFactory, slaveSimulatorFactory);
        }

        if (descriptor.Transport is IpTransportDescriptor ip)
        {
            if (ip.TransportType == IpTransportType.Tcp)
                return new ModbusTcpSimulationNetwork(ip, modbusFactory, slaveSimulatorFactory);

            if (ip.TransportType == IpTransportType.Udp)
                return new ModbusUdpSimulationNetwork(ip, modbusFactory, slaveSimulatorFactory);
        }


        throw new InvalidOperationException($"不支持的描述符: {descriptor}");
    }
}