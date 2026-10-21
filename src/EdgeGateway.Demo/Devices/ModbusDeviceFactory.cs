using EdgeGateway.Bytes.Codecs;
using EdgeGateway.Channels;
using EdgeGateway.Tags;
using NModbus;
using System.Buffers.Binary;

namespace EdgeGateway.Devices;


public sealed class ModbusDeviceFactory(IModbusFactory modbusFactory) : IDeviceFactory
{
    public bool CanCreate(IDeviceDescriptor descriptor)
    {
        return descriptor is ModbusDeviceDescriptor modbus;
    }
    public IDevice Create(IDeviceDescriptor descriptor, IByteChannel byteChannel)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(byteChannel);

        if (descriptor is not ModbusDeviceDescriptor modbus)
            throw new ArgumentException("描述不是 Modbus RTU。", nameof(descriptor));

        if (!Enum.IsDefined(modbus.FrameFormat))
            throw new ArgumentOutOfRangeException(nameof(descriptor));

        var streamSource = new StreamResourceAdapter(byteChannel);
        var modbusMaster = modbus.FrameFormat switch
        {
            ModbusFrameFormat.Rtu => modbusFactory.CreateRtuMaster(streamSource),
            ModbusFrameFormat.Ascii => modbusFactory.CreateAsciiMaster(streamSource),
            ModbusFrameFormat.Ip => modbusFactory.CreateIpMaster(streamSource),
            _ => throw new InvalidOperationException(),
        };

        return new ModbusDevice(modbusMaster);
    }
}

public sealed class ModbusDevice(IModbusMaster master) : DeviceBase
{
    protected override async ValueTask<T> ReadTagAsync<T>(ITag<T> tag, CancellationToken cancellationToken = default)
    {
        if (tag.Address is not ModbusDataAddress mosbus)
            throw new InvalidOperationException("不是有效的Modbus地址");


        if (mosbus.DataKind == DataKind.Coil)
        {
            var bits = await master.ReadCoilsAsync(mosbus.SlaveId, mosbus.StartAddress, mosbus.Quantity);
            return DecodeBits(tag.Codec, bits);
        }
        if (mosbus.DataKind == DataKind.DiscreteInput)
        {
            var bits = await master.ReadInputsAsync(mosbus.SlaveId, mosbus.StartAddress, mosbus.Quantity);
            return DecodeBits(tag.Codec, bits);
        }

        if (mosbus.DataKind == DataKind.HoldingRegister)
        {
            var words = await master.ReadHoldingRegistersAsync(mosbus.SlaveId, mosbus.StartAddress, mosbus.Quantity);
            return DecodeWords(tag.Codec, words);
        }
        if (mosbus.DataKind == DataKind.InputRegister)
        {
            var words = await master.ReadInputRegistersAsync(mosbus.SlaveId, mosbus.StartAddress, mosbus.Quantity);
            return DecodeWords(tag.Codec, words);
        }

        throw new InvalidProgramException();
    }

    protected override async ValueTask WriteTagAsync<T>(ITag<T> tag, T value, CancellationToken cancellationToken = default)
    {
        if (tag.Address is not ModbusDataAddress modbus)
            throw new InvalidOperationException("不是有效的Modbus地址");


        if (modbus.DataKind == DataKind.Coil)
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

        if (modbus.DataKind == DataKind.HoldingRegister)
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


    private static T DecodeBits<T>(IByteCodec<T> codec, bool[] bits)
    {
        ArgumentNullException.ThrowIfNull(codec);
        ArgumentNullException.ThrowIfNull(bits);

        var byteLength = (bits.Length + 7) / 8;
        Span<byte> cache = stackalloc byte[byteLength];

        for (int i = 0; i < bits.Length; i++)
        {
            if (bits[i])
                cache[i / 8] |= (byte)(1 << (i % 8));
        }

        return codec.Decode(cache);
    }

    private static T DecodeWords<T>(IByteCodec<T> codec, ushort[] words)
    {
        ArgumentNullException.ThrowIfNull(codec);
        ArgumentNullException.ThrowIfNull(words);

        Span<byte> cache = stackalloc byte[words.Length * 2];

        for (int i = 0; i < words.Length; i++)
            BinaryPrimitives.WriteUInt16BigEndian(cache[(i * 2)..], words[i]);

        return codec.Decode(cache);
    }
}