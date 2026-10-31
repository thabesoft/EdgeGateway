namespace ThabeSoft.EdgeGateway.Simulator;


public interface IModbusSimulator
{
    byte SlaveId { get; }

    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);


    void SetHoldingRegister(ushort address, ushort value);
    ushort GetHoldingRegister(ushort address);
}