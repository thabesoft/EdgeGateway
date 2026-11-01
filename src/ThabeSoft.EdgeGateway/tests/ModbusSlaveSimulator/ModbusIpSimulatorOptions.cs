using System.Net;

namespace ModbusSlaveSimulator;

public sealed class ModbusIpSimulatorOptions : ModbusSimulatorOptions
{
    public required IPEndPoint IPEndPoint { get; set; }
}