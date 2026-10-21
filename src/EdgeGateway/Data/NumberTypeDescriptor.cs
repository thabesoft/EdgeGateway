using EdgeGateway.Codecs;
using FluentResults;

namespace EdgeGateway.Data;

public sealed record class NumberTypeDescriptor<T> : ICodec<T>
#if NETSTANDARD2_0
    where T : unmanaged
#else
    where T : unmanaged, System.Numerics.INumber<T>
#endif
{
    private static readonly ICodec<T> _typeDescriptor = UnmanagedTypeDescriptor<T>.Instance;

    public string Name => _typeDescriptor.Name;
    public int? ByteLength => _typeDescriptor.ByteLength;


    public string? Unit { get; init; }



#if NETSTANDARD2_0
    private dynamic _scale = 1;
    private dynamic _offset = 0;

    public T Scale { get => _scale; set => _scale = value; }
    public T Offset { get => _offset; set => _offset = value; }
#else
    public T Scale { get; init; } = T.One;
    public T Offset { get; init; } = T.Zero;
#endif


    public NumberTypeDescriptor()
    {
#if NETSTANDARD2_0
        if (!IsNumericType(typeof(T))) throw new NotSupportedException("不是有效的数字类型");
#endif
    }


    public Result<T> Decode(ReadOnlySpan<byte> rawData)
    {
        var result = _typeDescriptor.Decode(rawData);
        if (result.IsFailed) return result;

        var value = result.Value;
#if NETSTANDARD2_0
        return (value * _scale) + _offset;
#else
        return (value * Scale) + Offset;
#endif
    }

    public Result<int> Encode(T value, Span<byte> buffer)
    {
#if NETSTANDARD2_0
        if (!IsNumericType(typeof(T)))
            return Result.Fail<int>($"类型 {typeof(T).Name} 不支持数值运算");

        var newValue = (T)((value - _offset) / _scale);
#else
        var newValue = (value - Offset) / Scale;
#endif

        return _typeDescriptor.Encode(newValue, buffer);
    }

#if NETSTANDARD2_0
    private static bool IsNumericType(Type type)
    {
        return type == typeof(byte) || type == typeof(sbyte) ||
               type == typeof(short) || type == typeof(ushort) ||
               type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) ||
               type == typeof(float) || type == typeof(double) ||
               type == typeof(decimal);
    }
#endif
}
