using ThabeSoft.EdgeGateway.Simulation.Networks;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;

public static class ModbusExtensions
{
    extension(Modbus)
    {
        public static ModbusSimulatorNetworkDescriptor RtuSimulationNetwork(SerialTransportDescriptor transportDescriptor) =>
            ModbusSimulatorNetworkDescriptor.Create(transportDescriptor, Modbus.Rtu);
        public static ModbusSimulatorNetworkDescriptor AsciiSimulationNetwork(SerialTransportDescriptor transportDescriptor) =>
            ModbusSimulatorNetworkDescriptor.Create(transportDescriptor, Modbus.Ascii);
        public static ModbusSimulatorNetworkDescriptor IpSimulationNetwork(IpTransportDescriptor transportDescriptor) =>
            ModbusSimulatorNetworkDescriptor.Create(transportDescriptor, Modbus.Ip);
    }
}
