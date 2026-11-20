using System.Collections;
using ThabeSoft.Primitives;

namespace ThabeSoft.EdgeGateway.Codecs;


public abstract record class CodecDescriptor
{
    public abstract DataType DataType { get; }
}

public abstract record class CodecDescriptor<T> : CodecDescriptor
{
    public override DataType DataType { get; } = GetKind();


    private static DataType GetKind()
    {
        if (TypesKind.TryGetValue(typeof(T), out var kind))
            return kind;

        throw new InvalidOperationException("不支持的编码器值类型");
    }
    private static Dictionary<Type, DataType> TypesKind { get; } = new()
    {
        { typeof(bool), DataType.Bool },
        { typeof(byte), DataType.Byte },

        { typeof(ushort), DataType.UInt16 },
        { typeof(short), DataType.Int16 },
        { typeof(char), DataType.Char },

        { typeof(uint), DataType.UInt32 },
        { typeof(int), DataType.Int32 },
        { typeof(float), DataType.Float },

        { typeof(ulong), DataType.UInt64 },
        { typeof(long), DataType.Int64 },
        { typeof(double), DataType.Double },

        { typeof(BitArray), DataType.Bits },
        { typeof(string), DataType.String }
    };
}