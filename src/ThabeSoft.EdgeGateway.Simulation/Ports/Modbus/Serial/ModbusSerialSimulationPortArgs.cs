using NModbus;
using ThabeSoft.EdgeGateway.Primitives.Guard;
using ThabeSoft.EdgeGateway.Serial;

namespace ThabeSoft.EdgeGateway.Ports.Modbus.Serial;


internal readonly struct ModbusSerialSimulationPortArgs
{
    public required SimulationPortBaseArgs Base { get; init => field = value.Required(); }
    public required SerialTransportDescriptor Transport { get; init => field = value.Required(); }
    public required IModbusFactory ModbusFactory { get; init => field = value.Required(); }
}