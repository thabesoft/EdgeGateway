using NModbus;
using System.Buffers.Binary;

namespace ThabeSoft.EdgeGateway.Tags;

internal sealed class ModbusTagWriter(IModbusMaster master) : ITagWriter
{
    public async ValueTask WriteAsync<T>(ITag<T> tag, T value, CancellationToken cancellationToken = default)
        where T : notnull
    {
        if (tag.Address is not ModbusDataAddress modbus)
            throw new InvalidOperationException("不是有效的Modbus地址");


        if (modbus.DataKind == ModbusDataKind.Coil)
        {
            Span<byte> cache = stackalloc byte[(modbus.Quantity + 7) / 8];
            tag.Codec.Encode(value, cache);


            if (modbus.Quantity == 1)
            {
                bool coil = (cache[0] & 1) != 0;
                await master.WriteSingleCoilAsync(modbus.SlaveId, modbus.StartAddress, coil);
            }
            else
            {
                var bits = new bool[modbus.Quantity];
                // ToBtis
                for (int i = 0; i < bits.Length; i++) bits[i] = (cache[i / 8] & (1 << (i % 8))) != 0;
                // Write
                await master.WriteMultipleCoilsAsync(modbus.SlaveId, modbus.StartAddress, bits);
            }
            return;
        }

        if (modbus.DataKind == ModbusDataKind.HoldingRegister)
        {
            Span<byte> cache = stackalloc byte[modbus.Quantity * 2];
            tag.Codec.Encode(value, cache);

            // Words
            var words = new ushort[modbus.Quantity];
            for (int i = 0; i < words.Length; i++)
                words[i] = BinaryPrimitives.ReadUInt16BigEndian(cache[(i * 2)..]);

            // Write
            if (words.Length == 1)
                await master.WriteSingleRegisterAsync(modbus.SlaveId, modbus.StartAddress, words[0]);
            else
                await master.WriteMultipleRegistersAsync(modbus.SlaveId, modbus.StartAddress, words);
            return;
        }

        throw new InvalidOperationException("不可写的数据类别。");
    }
}