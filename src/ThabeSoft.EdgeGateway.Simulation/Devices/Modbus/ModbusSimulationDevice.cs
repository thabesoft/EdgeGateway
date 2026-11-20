using Microsoft.Extensions.Logging;
using NModbus;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Primitives.Guard;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Simulation.Devices.Modbus;


internal sealed class ModbusSimulationDevice(in ModbusSimulationDeviceArgs args) : SimulationDeviceBase(args.Base)
{
    private readonly byte _unitId = args.UnitId;
    private readonly ILogger _logger = args.Base.Logger;
    private readonly ICodecFactory _codecFactory = args.CodecFactory;


    public IModbusSlave Slave { get; } = args.ModbusFactory.CreateSlave(args.UnitId);


    protected override void OnValue<T>(TagDescriptor<T> tag, T value)
    {
        if (tag.Address is not ModbusDataAddress address)
            return;

        var codec = _codecFactory.Create(tag.Codec);

        if (address.Kind == ModbusDataKind.HoldingRegister)
        {
            var words = codec.EncodeWords(value);
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


internal readonly struct ModbusSimulationDeviceArgs
{
    public required SimulationDeviceBaseArgs Base { get; init => field = value.Required(); }
    public required IModbusFactory ModbusFactory { get; init => field = value.Required(); }
    public required ICodecFactory CodecFactory { get; init => field = value.Required(); }
    public required byte UnitId { get; init => field = value.Positive(); }
}