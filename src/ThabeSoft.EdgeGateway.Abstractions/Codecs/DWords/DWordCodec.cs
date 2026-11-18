using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.DWords;


public abstract record class DWordCodecDescriptor<TDWord> : CodecDescriptor<TDWord>, IRequiredBits
{
    static DWordCodecDescriptor()
    {
        var type = typeof(TDWord);
        if (type != typeof(uint) && type != typeof(int) && type != typeof(float))
        {
            throw new InvalidOperationException($"类型 {type.Name} 不是有效的 DWord 类型(uint/int/float)");
        }
    }

    public int RequiredBits { get; } = 8 * 2;
    public required DWordLayout Layout { get; init => field.Defined(); }
}


public sealed record class UInt32CodecDescriptor : DWordCodecDescriptor<uint>
{
    private UInt32CodecDescriptor() { }

    public static UInt32CodecDescriptor Abcd { get; } = new() { Layout = DWordLayout.Abcd };
    public static UInt32CodecDescriptor Badc { get; } = new() { Layout = DWordLayout.Badc };
    public static UInt32CodecDescriptor Cdab { get; } = new() { Layout = DWordLayout.Cdab };
    public static UInt32CodecDescriptor Dcba { get; } = new() { Layout = DWordLayout.Dcba };

    public static UInt32CodecDescriptor Create(DWordLayout layout)
    {
        return layout switch
        {
            DWordLayout.Abcd => Abcd,
            DWordLayout.Badc => Badc,
            DWordLayout.Cdab => Cdab,
            DWordLayout.Dcba => Dcba,
            _ => throw new InvalidOperationException($"不支持的 UInt32 布局 ({layout})")
        };
    }
}


public sealed record class Int32CodecDescriptor : DWordCodecDescriptor<int>
{
    private Int32CodecDescriptor() { }

    public static Int32CodecDescriptor Abcd { get; } = new() { Layout = DWordLayout.Abcd };
    public static Int32CodecDescriptor Badc { get; } = new() { Layout = DWordLayout.Badc };
    public static Int32CodecDescriptor Cdab { get; } = new() { Layout = DWordLayout.Cdab };
    public static Int32CodecDescriptor Dcba { get; } = new() { Layout = DWordLayout.Dcba };

    public static Int32CodecDescriptor Create(DWordLayout layout)
    {
        return layout switch
        {
            DWordLayout.Abcd => Abcd,
            DWordLayout.Badc => Badc,
            DWordLayout.Cdab => Cdab,
            DWordLayout.Dcba => Dcba,
            _ => throw new InvalidOperationException($"不支持的 Int32 布局 ({layout})")
        };
    }
}

public sealed record class FloatCodecDescriptor : DWordCodecDescriptor<float>
{
    private FloatCodecDescriptor() { }

    public static FloatCodecDescriptor Abcd { get; } = new() { Layout = DWordLayout.Abcd };
    public static FloatCodecDescriptor Badc { get; } = new() { Layout = DWordLayout.Badc };
    public static FloatCodecDescriptor Cdab { get; } = new() { Layout = DWordLayout.Cdab };
    public static FloatCodecDescriptor Dcba { get; } = new() { Layout = DWordLayout.Dcba };

    public static FloatCodecDescriptor Create(DWordLayout layout)
    {
        return layout switch
        {
            DWordLayout.Abcd => Abcd,
            DWordLayout.Badc => Badc,
            DWordLayout.Cdab => Cdab,
            DWordLayout.Dcba => Dcba,
            _ => throw new InvalidOperationException($"不支持的 Float 布局 ({layout})")
        };
    }
}