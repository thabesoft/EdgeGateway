using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NModbus;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Devices.Modbus;


internal sealed class ModbusSimulationDevice(
        ModbusSimulationDeviceId id,
        IModbusFactory modbusFactory,
        TriggerManager triggerManager,
        ILogger<ModbusSimulationDevice>? logger = null
    ) : SimulationDeviceBase(id, triggerManager, logger)
{

    private readonly ILogger<ModbusSimulationDevice> _logger = logger ?? NullLogger<ModbusSimulationDevice>.Instance;
    private byte _unitId = id.Value;

    public IModbusSlave Slave { get; } = modbusFactory.CreateSlave(id.Value);


    protected override void OnValue<T>(TagDescriptor<T> tag, T value)
    {
        if (tag.Address is not ModbusDataAddress address)
            return;

        if (address.Kind == ModbusDataKind.HoldingRegister)
        {
            var words = tag.Codec.EncodeWords(value);
            SetHoldingRegisters(address.StartAddress, words);
        }
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
