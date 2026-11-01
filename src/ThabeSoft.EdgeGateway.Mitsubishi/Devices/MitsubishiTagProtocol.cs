//using ThabeSoft.EdgeGateway.Byte.Codecs;
//using ThabeSoft.EdgeGateway.Device;
//using ThabeSoft.EdgeGateway.Tag;
//using System.Buffers.Binary;

//namespace ThabeSoft.EdgeGateway.Mitsubishi.Device;


//internal sealed class MitsubishiTagProtocol(IModbusMaster master) : ITagReader, ITagWriter
//{
//    public async ValueTask<T> ReadAsync<T>(ITag<T> tag, CancellationToken cancellationToken = default)
//        where T : notnull
//    {
//        if (tag.Address is not ModbusDataAddress mosbus)
//            throw new InvalidOperationException("不是有效的Modbus地址");


//        if (mosbus.DataKind == ModbusDataKind.Coil)
//        {
//            var bits = await master.ReadCoilsAsync(mosbus.SlaveId, mosbus.StartAddress, mosbus.Quantity);
//            return DecodeBits(tag.Codec, bits);
//        }
//        if (mosbus.DataKind == ModbusDataKind.DiscreteInput)
//        {
//            var bits = await master.ReadInputsAsync(mosbus.SlaveId, mosbus.StartAddress, mosbus.Quantity);
//            return DecodeBits(tag.Codec, bits);
//        }

//        if (mosbus.DataKind == ModbusDataKind.HoldingRegister)
//        {
//            var words = await master.ReadHoldingRegistersAsync(mosbus.SlaveId, mosbus.StartAddress, mosbus.Quantity);
//            return DecodeWords(tag.Codec, words);
//        }
//        if (mosbus.DataKind == ModbusDataKind.InputRegister)
//        {
//            var words = await master.ReadInputRegistersAsync(mosbus.SlaveId, mosbus.StartAddress, mosbus.Quantity);
//            return DecodeWords(tag.Codec, words);
//        }

//        throw new InvalidProgramException();
//    }
//    public async ValueTask WriteAsync<T>(ITag<T> tag, T value, CancellationToken cancellationToken = default)
//        where T : notnull
//    {
//        if (tag.Address is not ModbusDataAddress modbus)
//            throw new InvalidOperationException("不是有效的Modbus地址");


//        if (modbus.DataKind == ModbusDataKind.Coil)
//        {
//            Span<byte> cache = stackalloc byte[(modbus.Quantity + 7) / 8];
//            tag.Codec.Encode(value, cache);


//            if (modbus.Quantity == 1)
//            {
//                bool coil = (cache[0] & 1) != 0;
//                await master.WriteSingleCoilAsync(modbus.SlaveId, modbus.StartAddress, coil);
//            }
//            else
//            {
//                var bits = new bool[modbus.Quantity];
//                // ToBtis
//                for (int i = 0; i < bits.Length; i++) bits[i] = (cache[i / 8] & (1 << (i % 8))) != 0;
//                // Write
//                await master.WriteMultipleCoilsAsync(modbus.SlaveId, modbus.StartAddress, bits);
//            }
//            return;
//        }

//        if (modbus.DataKind == ModbusDataKind.HoldingRegister)
//        {
//            Span<byte> cache = stackalloc byte[modbus.Quantity * 2];
//            tag.Codec.Encode(value, cache);

//            // Words
//            var words = new ushort[modbus.Quantity];
//            for (int i = 0; i < words.Length; i++)
//                words[i] = BinaryPrimitives.ReadUInt16BigEndian(cache[(i * 2)..]);

//            // Write
//            if (words.Length == 1)
//                await master.WriteSingleRegisterAsync(modbus.SlaveId, modbus.StartAddress, words[0]);
//            else
//                await master.WriteMultipleRegistersAsync(modbus.SlaveId, modbus.StartAddress, words);
//            return;
//        }

//        throw new InvalidOperationException("不可写的数据类别。");
//    }


//    private static T DecodeBits<T>(IByteCodec<T> codec, bool[] bits)
//        where T : notnull
//    {
//        ArgumentNullException.ThrowIfNull(codec);
//        ArgumentNullException.ThrowIfNull(bits);

//        var byteLength = (bits.Length + 7) / 8;
//        Span<byte> cache = stackalloc byte[byteLength];

//        for (int i = 0; i < bits.Length; i++)
//        {
//            if (bits[i])
//                cache[i / 8] |= (byte)(1 << (i % 8));
//        }

//        return codec.Decode(cache);
//    }
//    private static T DecodeWords<T>(IByteCodec<T> codec, ushort[] words)
//        where T : notnull
//    {
//        ArgumentNullException.ThrowIfNull(codec);
//        ArgumentNullException.ThrowIfNull(words);

//        Span<byte> cache = stackalloc byte[words.Length * 2];

//        for (int i = 0; i < words.Length; i++)
//            BinaryPrimitives.WriteUInt16BigEndian(cache[(i * 2)..], words[i]);

//        return codec.Decode(cache);
//    }
//}