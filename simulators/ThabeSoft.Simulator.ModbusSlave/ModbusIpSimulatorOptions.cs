using System.Net;

namespace ThabeSoft.Simulator.ModbusSlave;

public sealed class ModbusIpSimulatorOptions : ModbusSimulatorOptions
{
    public required IPEndPoint IPEndPoint { get; set; }
}