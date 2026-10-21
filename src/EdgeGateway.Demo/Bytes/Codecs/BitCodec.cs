using System.Collections;

namespace EdgeGateway.Bytes.Codecs;

public sealed class BitCodec : IByteCodec<BitArray>
{
    public static BitCodec LsbFirst(int bitCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bitCount);
        return new(bitCount, BitOrder.LsbFirst);
    }
    public static BitCodec MsbFirst(int bitCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bitCount);
        return new(bitCount, BitOrder.MsbFirst);
    }



    private readonly int _bitCount;
    private readonly BitOrder _bitOrder;

    public int ByteLength => (_bitCount + 7) / 8;


    private BitCodec(int bitCount, BitOrder bitOrder)
    {
        _bitCount = bitCount;
        _bitOrder = bitOrder;
    }


    public int Encode(BitArray value, Span<byte> buffer)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length != _bitCount)
            throw new ArgumentException($"BitArray 长度必须为 {_bitCount}。", nameof(value));

        if (buffer.Length < ByteLength)
            throw new ArgumentException($"需要至少 {ByteLength} 字节。", nameof(buffer));


        buffer[..ByteLength].Clear();

        for (int i = 0; i < _bitCount; i++)
        {
            if (!value[i]) continue;
            int byteIndex = i / 8;
            int bitIndex = _bitOrder == BitOrder.LsbFirst ? i % 8 : 7 - (i % 8);
            buffer[byteIndex] |= (byte)(1 << bitIndex);
        }
        return ByteLength;
    }

    public BitArray Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < ByteLength)
            throw new ArgumentException($"需要至少 {ByteLength} 字节。", nameof(raw));

        var bits = new BitArray(_bitCount);
        for (int i = 0; i < _bitCount; i++)
        {
            int byteIndex = i / 8;
            int bitIndex = _bitOrder == BitOrder.LsbFirst ? i % 8 : 7 - (i % 8);
            bits[i] = (raw[byteIndex] & (1 << bitIndex)) != 0;
        }
        return bits;
    }
}

public static class BitPacker
{
    public static int ToBytes(ReadOnlySpan<bool> bits, Span<byte> buffer)
    {
        var byteLength = (bits.Length + 7) / 8;
        if (buffer.Length < byteLength)
            throw new ArgumentException($"缓冲区长度必须大于 {byteLength}。", nameof(buffer));

        buffer[..byteLength].Clear();

        for (int i = 0; i < bits.Length; i++)
        {
            if (bits[i])
                buffer[i / 8] |= (byte)(1 << (i % 8));
        }

        return byteLength;
    }
}


public enum BitOrder
{
    LsbFirst,
    MsbFirst
}