using System.Collections;

namespace ThabeSoft.EdgeGateway.Codecs;


public abstract record class CodecDescriptor
{
    public abstract CodecKind Kind { get; }
}

public abstract record class CodecDescriptor<T> : CodecDescriptor
{
    public override CodecKind Kind { get; } = GetKind();


    private static CodecKind GetKind()
    {
        if (TypesKind.TryGetValue(typeof(T), out var kind))
            return kind;

        throw new InvalidOperationException("不支持的编码器值类型");
    }
    private static Dictionary<Type, CodecKind> TypesKind { get; } = new()
    {
        { typeof(bool), CodecKind.Bool },
        { typeof(byte), CodecKind.Byte },

        { typeof(ushort), CodecKind.UInt16 },
        { typeof(short), CodecKind.Int16 },
        { typeof(char), CodecKind.Char },

        { typeof(uint), CodecKind.UInt32 },
        { typeof(int), CodecKind.Int32 },
        { typeof(float), CodecKind.Float },

        { typeof(ulong), CodecKind.UInt64 },
        { typeof(long), CodecKind.Int64 },
        { typeof(double), CodecKind.Double },

        { typeof(BitArray), CodecKind.Bits },
        { typeof(string), CodecKind.String }
    };
}