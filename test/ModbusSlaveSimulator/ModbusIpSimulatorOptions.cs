using System.Net;

namespace ThabeSoft.ModbusSimulator;

public sealed class ModbusIpSimulatorOptions : ModbusSimulatorOptions
{
    public required IPEndPoint IPEndPoint { get; set; }
}