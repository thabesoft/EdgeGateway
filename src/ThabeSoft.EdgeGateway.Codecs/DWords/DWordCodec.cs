using System.Buffers.Binary;

namespace ThabeSoft.EdgeGateway.Codecs.DWords;


internal sealed class DWordCodec : ICodec<uint>
{
    public static DWordCodec Abcd { get; } = new DWordCodec(DWordSwapper.Abcd);
    public static DWordCodec Badc { get; } = new DWordCodec(DWordSwapper.Badc);
    public static DWordCodec Dcba { get; } = new DWordCodec(DWordSwapper.Dcba);
    public static DWordCodec Cdab { get; } = new DWordCodec(DWordSwapper.Cdab);


    public static DWordCodec Create(DWordLayout layout)
    {
        return layout switch
        {
            DWordLayout.Abcd => Abcd,
            DWordLayout.Badc => Badc,
            DWordLayout.Cdab => Cdab,
            DWordLayout.Dcba => Dcba,
            _ => throw new InvalidOperationException($"无法创建编码器, 不支持的布局 ({layout})")
        };
    }



    private readonly DWordSwapper _swapper;
    private DWordCodec(DWordSwapper swapper) => _swapper = swapper;


    public const int ByteLength = 4;

    public int Encode(uint value, Span<byte> buffer)
    {
        if (buffer.Length < ByteLength)
            throw new ArgumentException($"缓冲区至少需要 {ByteLength} 字节。", nameof(buffer));

        BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
        _swapper.Swap(buffer);

        return ByteLength;
    }

    public uint Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length < ByteLength)
            throw new ArgumentException($"数据至少需要 {ByteLength} 字节。", nameof(raw));

        Span<byte> cache = stackalloc byte[ByteLength];
        raw.CopyTo(cache);
        _swapper.Swap(cache);

        return BinaryPrimitives.ReadUInt32BigEndian(cache);
    }
}

internal sealed class FloatCodec(ICodec<uint> Codec) : ICodec<float>
{
    public int Encode(float value, Span<byte> buffer) => Codec.Encode(BitConverter.SingleToUInt32Bits(value), buffer);
    public float Decode(ReadOnlySpan<byte> raw) => BitConverter.UInt32BitsToSingle(Codec.Decode(raw));
}

internal sealed class IntCodec(ICodec<uint> Codec) : ICodec<int>
{
    public int Encode(int value, Span<byte> buffer) => Codec.Encode((uint)value, buffer);
    public int Decode(ReadOnlySpan<byte> raw) => (int)Codec.Decode(raw);
}