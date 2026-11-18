using NModbus;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Ports.Modbus.Ip;

internal readonly struct ModbusIpSimulationPortArgs
{
    public required SimulationPortBaseArgs Base { get; init => field = value.Required(); }
    public required IpTransportDescriptor Transport { get; init => field = value.Required(); }
    public required IModbusFactory ModbusFactory { get; init => field = value.Required(); }
}