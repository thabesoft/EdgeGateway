using System.Collections;

namespace ThabeSoft.EdgeGateway.Codecs.Internal;


internal sealed class BitsCodec : ICodec<BitArray>, IRequiredBits
{
    public BitsCodec(int bitCount, BitOrder bitOrder)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bitCount);
        if (BitOrder is not (BitOrder.LsbFirst or BitOrder.MsbFirst))
            throw new ArgumentOutOfRangeException(nameof(bitOrder), bitCount, "不支持的位序");

        BitOrder = bitOrder;
        RequiredBits = bitCount;
        RequiredBytes = (bitCount + 7) / 8;
    }


    public BitOrder BitOrder { get; }
    public int RequiredBytes { get; }
    public int RequiredBits { get; }


    public int Encode(BitArray value, Span<byte> buffer)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length != RequiredBits)
            throw new ArgumentException($"BitArray 长度必须为 {RequiredBits}。", nameof(value));

        if (buffer.Length < RequiredBytes)
            throw new ArgumentException($"需要至少 {RequiredBytes} 字节。", nameof(buffer));


        buffer[..RequiredBytes].Clear();

        for (int i = 0; i < RequiredBits; i++)
        {
            if (!value[i]) continue;
            int byteIndex = i / 8;
            int bitIndex = BitOrder == BitOrder.LsbFirst ? i % 8 : 7 - (i % 8);
            buffer[byteIndex] |= (byte)(1 << bitIndex);
        }
        return RequiredBytes;
    }
    public BitArray Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < RequiredBytes)
            throw new ArgumentException($"需要至少 {RequiredBytes} 字节。", nameof(raw));

        var bits = new BitArray(RequiredBits);
        for (int i = 0; i < RequiredBits; i++)
        {
            int byteIndex = i / 8;
            int bitIndex = BitOrder == BitOrder.LsbFirst ? i % 8 : 7 - (i % 8);
            bits[i] = (raw[byteIndex] & (1 << bitIndex)) != 0;
        }
        return bits;
    }


    int ICodec.Encode(object value, Span<byte> buffer) => Encode((BitArray)value, buffer);
    object ICodec.Decode(ReadOnlySpan<byte> raw) => Decode(raw);
}


internal enum BitOrder
{
    LsbFirst,
    MsbFirst
}