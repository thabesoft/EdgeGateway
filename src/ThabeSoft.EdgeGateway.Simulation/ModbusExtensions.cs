
<<<<<<< TODO: 项目“ThabeSoft.EdgeGateway.Simulation (netstandard2.0)”的未合并的更改, 在此之前:
using ThabeSoft.EdgeGateway.Simulation.Networks;
=======
using ThabeSoft.EdgeGateway.Modbus;
using ThabeSoft.EdgeGateway.Simulation.Networks;
>>>>>>> 晚于

<<<<<<< TODO: 项目“ThabeSoft.EdgeGateway.Simulation (net8.0)”的未合并的更改, 在此之前:
using ThabeSoft.EdgeGateway.Simulation.Networks;
=======
using ThabeSoft.EdgeGateway.Modbus;
using ThabeSoft.EdgeGateway.Modbus.Modbus;
using ThabeSoft.EdgeGateway.Modbus.Modbus.Modbus;
using ThabeSoft.EdgeGateway.Simulation.Networks;
>>>>>>> 晚于

<<<<<<< TODO: 项目“ThabeSoft.EdgeGateway.Simulation (net11.0)”的未合并的更改, 在此之前:
using ThabeSoft.EdgeGateway.Simulation.Networks;
=======
using ThabeSoft.EdgeGateway.Modbus;
using ThabeSoft.EdgeGateway.Modbus.Modbus;
using ThabeSoft.EdgeGateway.Simulation.Networks;
>>>>>>> 晚于
using ThabeSoft.EdgeGateway.Modbus.ModbusProtocols;
using ThabeSoft.EdgeGateway.Modbus.ModbusProtocols.Modbus;
using ThabeSoft.EdgeGateway.Modbus.ModbusProtocols.Modbus.Modbus;
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
