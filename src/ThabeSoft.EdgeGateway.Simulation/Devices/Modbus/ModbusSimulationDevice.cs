using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NModbus;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Devices.Modbus;


internal sealed class ModbusSimulationDevice(
        ModbusSimulationDeviceDescriptor descriptor,
        IModbusFactory modbusFactory,
        ITriggerFactorySelector triggerFactorySelector,
        ILogger<ModbusSimulationDevice>? logger = null
    ) : SimulationDeviceBase(descriptor, triggerFactorySelector, logger)
{

    private readonly ILogger<ModbusSimulationDevice> _logger = logger ?? NullLogger<ModbusSimulationDevice>.Instance;
    private byte _unitId = descriptor.Id.Value;

    public IModbusSlave Slave { get; } = modbusFactory.CreateSlave(descriptor.Id.Value);


    protected override ValueTask OnValueAsync<T>(TagDescriptor<T> tag, T value, CancellationToken cancellationToken)
    {
        if (tag.Address is not ModbusDataAddress address)
            return ValueTask.CompletedTask;

        if (address.Kind == ModbusDataKind.HoldingRegister)
        {
            var words = tag.Codec.EncodeWords(value);
            SetHoldingRegisters(address.StartAddress, words);
        }

        return ValueTask.CompletedTask;
    }


    private void SetCoils(ushort address, params bool[] values)
    {
        Slave.DataStore.CoilInputs.WritePoints(address, values);
        _logger.LogTrace("Slave[{Id}] 设置线圈值: ({Address},{values})", _unitId, address, string.Join(string.Empty, values.Select(x => x ? '1' : '0')));
    }
    private void SetHoldingRegisters(ushort address, params ushort[] values)
    {
        Slave.DataStore.HoldingRegisters.WritePoints(address, values);
        _logger.LogTrace("Slave[{Id}] 设置保持寄存器值: ({Address},{values})", _unitId, address, string.Join(" ", values));
    }
}
