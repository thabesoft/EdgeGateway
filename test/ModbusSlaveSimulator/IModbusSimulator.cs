namespace ThabeSoft.ModbusSimulator;


public interface IModbusSimulator
{
    byte UnitId { get; }

    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);


    void SetHoldingRegister(ushort address, ushort value);
    ushort GetHoldingRegister(ushort address);
}