using EdgeGateway.Byte.Codecs;
using EdgeGateway.Tag;
using NModbus;

namespace ThabeSoft.EdgeGateway.Modbus;


internal sealed class ModbusTagReader(IModbusMaster master) : ITagReader
{
    public async ValueTask<T> ReadAsync<T>(ITag<T> tag, CancellationToken cancellationToken = default)
        where T : notnull
    {
        if (tag.Address is not ModbusDataAddress mosbus)
            throw new InvalidOperationException("不是有效的Modbus地址");


        if (mosbus.DataKind == ModbusDataKind.Coil)
        {
            var bits = await master.ReadCoilsAsync(mosbus.SlaveId, mosbus.StartAddress, mosbus.Quantity);
            return tag.Codec.DecodeBits(bits);
        }
        if (mosbus.DataKind == ModbusDataKind.DiscreteInput)
        {
            var bits = await master.ReadInputsAsync(mosbus.SlaveId, mosbus.StartAddress, mosbus.Quantity);
            return tag.Codec.DecodeBits(bits);
        }

        if (mosbus.DataKind == ModbusDataKind.HoldingRegister)
        {
            var words = await master.ReadHoldingRegistersAsync(mosbus.SlaveId, mosbus.StartAddress, mosbus.Quantity);
            return tag.Codec.DecodeWords(words);
        }
        if (mosbus.DataKind == ModbusDataKind.InputRegister)
        {
            var words = await master.ReadInputRegistersAsync(mosbus.SlaveId, mosbus.StartAddress, mosbus.Quantity);
            return tag.Codec.DecodeWords(words);
        }

        throw new InvalidProgramException();
    }
}