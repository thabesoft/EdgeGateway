namespace ThabeSoft.EdgeGateway.Simulator;

public class ModbusSimulatorOptions
{
    public TimeSpan TickInterval { get; set; } = TimeSpan.FromMilliseconds(100);
    public byte UnitId { get; set; } = 1;
}
