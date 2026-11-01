using System.IO.Ports;

namespace ModbusSlaveSimulator;

public sealed class ModbusSerialSimulatorOptions : ModbusSimulatorOptions
{
    public required SerialPortOptions SerialPortOptions { get; set; }
}

public sealed class SerialPortOptions
{
    public required string PortName { get; set; }
    public int BaudRate { get; set; } = 9600;
    public Parity Parity { get; set; } = Parity.None;
    public int DataBits { get; set; } = 8;
    public StopBits StopBits { get; set; } = StopBits.One;
}