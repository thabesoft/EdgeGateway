using EdgeGateway.Data;
using EdgeGateway.Devices;
using EdgeGateway.Ports;
using FluentResults;
using NModbus;
using NModbus.IO;

namespace EdgeGateway.Modbus;


public sealed class ModbusDevice : IDevice
{
    private readonly IPort _port;
    private readonly IModbusMaster _master;

    public required DeviceId Id { get; init; }
    public PortAddress PortAddress => _port.Address;


    private ModbusDevice(IPort port, IModbusMaster master)
    {
        _port = port;
        _master = master;
    }
    public static ModbusDevice CreateRtu(IPort port, DeviceId id)
    {
        var master = new ModbusFactory().CreateRtuMaster(new PortStreamResourceAdpter(port));
        return new ModbusDevice(port, master) { Id = id };
    }
    public static ModbusDevice CreateAscii(IPort port, DeviceId id)
    {
        var master = new ModbusFactory().CreateAsciiMaster(new PortStreamResourceAdpter(port));
        return new ModbusDevice(port, master) { Id = id };
    }
    public static ModbusDevice CreateIp(IPort port, DeviceId id)
    {
        var master = new ModbusFactory().CreateIpMaster(new PortStreamResourceAdpter(port));
        return new ModbusDevice(port, master) { Id = id };
    }



    public ValueTask<Result> ConnectAsync(CancellationToken cancellationToken = default)
    {
        return new ValueTask<Result>(Result.Ok());
    }
    public ValueTask<Result> DisconnectAsync(CancellationToken cancellationToken = default)
    {
        return new ValueTask<Result>(Result.Ok());
    }
    public ValueTask DisposeAsync()
    {
        return default;
    }



    public async ValueTask<Result<T>> ReadAsync<T>(DataDescriptor<T> descriptor, CancellationToken cancellationToken = default)
    {
        if (descriptor.Address is not ModbusAddress address) return Result.Fail<T>("不支持的地址");

        try
        {
            if (address.Region is ModbusRegion.Coils or ModbusRegion.DiscreteInputs)
            {
                bool[] values = [];

                if (address.Region == ModbusRegion.Coils)
                {
                    values = await _master.ReadCoilsAsync(address.SlaveId, address.Address, address.Quantity);
                }
                else
                {
                    values = await _master.ReadInputsAsync(address.SlaveId, address.Address, address.Quantity);
                }

                Span<byte> buffer = stackalloc byte[(address.Quantity + 7) / 8];
                ConvertToBytes(values, buffer);
                return descriptor.TypeDescriptor.Decode(buffer);
            }

            if (address.Region is ModbusRegion.InputRegisters or ModbusRegion.HoldingRegisters)
            {
                ushort[] values = [];

                if (address.Region == ModbusRegion.HoldingRegisters)
                {
                    values = await _master.ReadHoldingRegistersAsync(address.SlaveId, address.Address, address.Quantity);
                }
                else
                {
                    values = await _master.ReadInputRegistersAsync(address.SlaveId, address.Address, address.Quantity);
                }

                Span<byte> buffer = stackalloc byte[address.Quantity * 2];
                ConvertToBytes(values, buffer);
                return descriptor.TypeDescriptor.Decode(buffer);
            }


            return Result.Fail<T>("不支持的功能码");
        }
        catch (Exception ex)
        {
            return new ExceptionalError(ex);
        }
    }
    public async ValueTask<Result> WriteAsync<T>(DataDescriptor<T> descriptor, T value, CancellationToken cancellationToken = default)
    {
        if (descriptor.Address is not ModbusAddress address) return Result.Fail("不支持的地址");

        try
        {
            Span<byte> buffer = stackalloc byte[descriptor.TypeDescriptor.ByteLength ?? 64];
            var encodeResult = descriptor.TypeDescriptor.Encode(value, buffer);
            if (!encodeResult.IsSuccess) return Result.Fail("解码失败").WithErrors(encodeResult.Errors);
            var encodedValue = buffer[..encodeResult.Value];


            if (address.Region is ModbusRegion.Coils)
            {
                Span<bool> values = stackalloc bool[((descriptor.TypeDescriptor.ByteLength + 7) / 8) ?? 8];
                ConvertToBools(encodedValue, values);

                if (values.Length == 1)
                {
                    await _master.WriteSingleCoilAsync(address.SlaveId, address.Address, values[0]);
                }
                else
                {
                    await _master.WriteMultipleCoilsAsync(address.SlaveId, address.Address, values.ToArray());
                }

                return Result.Ok();
            }

            if (address.Region is ModbusRegion.HoldingRegisters)
            {
                Span<ushort> values = stackalloc ushort[(descriptor.TypeDescriptor.ByteLength / 2) ?? 128];
                ConvertToUshorts(encodedValue, values);

                if (values.Length == 1)
                {
                    await _master.WriteSingleRegisterAsync(address.SlaveId, address.Address, values[0]);
                }
                else
                {
                    await _master.WriteMultipleRegistersAsync(address.SlaveId, address.Address, values.ToArray());
                }

                return Result.Ok();
            }

            return Result.Fail("不支持的功能码");
        }
        catch (Exception ex)
        {
            return new ExceptionalError(ex);
        }
    }



    private static void ConvertToBytes(ReadOnlySpan<bool> bits, Span<byte> destination)
    {
        int byteCount = (bits.Length + 7) / 8;
        if (destination.Length < byteCount)
            throw new ArgumentException("Destination span too small", nameof(destination));

        destination.Slice(0, byteCount).Clear();

        for (int i = 0; i < bits.Length; i++)
        {
            if (bits[i])
                destination[i / 8] |= (byte)(1 << (i % 8));
        }
    }
    private static void ConvertToBytes(ReadOnlySpan<ushort> registers, Span<byte> destination)
    {
        int byteCount = registers.Length * 2;
        if (destination.Length < byteCount)
            throw new ArgumentException("Destination span too small", nameof(destination));

        for (int i = 0; i < registers.Length; i++)
        {
            ushort value = registers[i];
            destination[i * 2] = (byte)(value & 0xFF);       // LSB
            destination[i * 2 + 1] = (byte)((value >> 8) & 0xFF); // MSB
        }
    }
    private static void ConvertToBools(ReadOnlySpan<byte> source, Span<bool> destination)
    {
        int bitCount = destination.Length;
        int byteCount = (bitCount + 7) / 8;
        if (source.Length < byteCount)
            throw new ArgumentException("Source span too small", nameof(source));

        for (int i = 0; i < bitCount; i++)
        {
            byte mask = (byte)(1 << (i % 8));
            destination[i] = (source[i / 8] & mask) != 0;
        }
    }
    private static void ConvertToUshorts(ReadOnlySpan<byte> source, Span<ushort> destination)
    {
        int wordCount = source.Length / 2;

        if (destination.Length < wordCount)
            throw new ArgumentException("Source span too small", nameof(source));

        for (int i = 0; i < wordCount; i++)
        {
            int byteIndex = i * 2;
            destination[i] = (ushort)(source[byteIndex] | (source[byteIndex + 1] << 8));
        }
    }
}


file sealed class PortStreamResourceAdpter(IPort port) : IStreamResource
{
    public int InfiniteTimeout { get; set; } = 1000;
    public int ReadTimeout { get; set; } = 1000;
    public int WriteTimeout { get; set; } = 1000;


    public int Read(byte[] buffer, int offset, int count)
    {
        var mem = buffer.AsMemory(offset, count);

        port.ReadExactlyAsync(mem).AsTask().GetAwaiter().GetResult();
        return mem.Length;
    }

    public void Write(byte[] buffer, int offset, int count)
    {
        var mem = buffer.AsMemory(offset, count);
        port.WriteAsync(mem).AsTask().GetAwaiter().GetResult();
    }
    public void Dispose()
    {
        port.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    public void DiscardInBuffer()
    {
        (port as IBufferDiscardable)?.DiscardInBuffer();
    }
}