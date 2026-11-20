using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.Words;


public abstract record class WordCodecDescriptor<TWord> : CodecDescriptor<TWord>, IRequiredBits
{
    static WordCodecDescriptor()
    {
        var type = typeof(TWord);
        if (type != typeof(short) && type != typeof(ushort) && type != typeof(char))
        {
            throw new InvalidOperationException($"类型 {type.Name} 不是有效的 Word 类型(short/ushort/char)");
        }
    }

    public int RequiredBits { get; } = 8 * 2;
    public required WordEndianess Endianess { get; init => field.Defined(); }
}


public sealed record class UInt16CodecDescriptor : WordCodecDescriptor<ushort>
{
    private UInt16CodecDescriptor() { }

    public static UInt16CodecDescriptor BigEndian { get; } = new() { Endianess = WordEndianess.Big };
    public static UInt16CodecDescriptor LittleEndian { get; } = new() { Endianess = WordEndianess.Little };

    public static UInt16CodecDescriptor Create(WordEndianess endianess)
    {
        return endianess switch
        {
            WordEndianess.Big => BigEndian,
            WordEndianess.Little => LittleEndian,
            _ => throw new InvalidOperationException($"不支持的 UInt16 字节序 ({endianess})")
        };
    }
}

public sealed record class Int16CodecDescriptor : WordCodecDescriptor<short>
{
    private Int16CodecDescriptor() { }

    public static Int16CodecDescriptor BigEndian { get; } = new() { Endianess = WordEndianess.Big };
    public static Int16CodecDescriptor LittleEndian { get; } = new() { Endianess = WordEndianess.Little };

    public static Int16CodecDescriptor Create(WordEndianess endianess)
    {
        return endianess switch
        {
            WordEndianess.Big => BigEndian,
            WordEndianess.Little => LittleEndian,
            _ => throw new InvalidOperationException($"不支持的 Int16 字节序 ({endianess})")
        };
    }
}


public sealed record class CharCodecDescriptor : WordCodecDescriptor<char>
{
    private CharCodecDescriptor() { }

    public static CharCodecDescriptor BigEndian { get; } = new() { Endianess = WordEndianess.Big };
    public static CharCodecDescriptor LittleEndian { get; } = new() { Endianess = WordEndianess.Little };

    public static CharCodecDescriptor Create(WordEndianess endianess)
    {
        return endianess switch
        {
            WordEndianess.Big => BigEndian,
            WordEndianess.Little => LittleEndian,
            _ => throw new InvalidOperationException($"不支持的 Char 字节序 ({endianess})")
        };
    }
}