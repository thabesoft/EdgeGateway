using System.Net;

namespace ThabeSoft.EdgeGateway.Simulator;

public sealed class ModbusIpSimulatorOptions : ModbusSimulatorOptions
{
    public required IPEndPoint IPEndPoint { get; set; }
}