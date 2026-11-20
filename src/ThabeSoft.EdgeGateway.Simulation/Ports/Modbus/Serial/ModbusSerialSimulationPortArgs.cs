using NModbus;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Transports.Serial;

namespace ThabeSoft.EdgeGateway.Ports.Modbus.Serial;


internal readonly struct ModbusSerialSimulationPortArgs
{
    public required SimulationPortBaseArgs Base { get; init => field = value.Required(); }
    public required SerialTransportDescriptor Transport { get; init => field = value.Required(); }
    public required IModbusFactory ModbusFactory { get; init => field = value.Required(); }
}