using EdgeGateway.Codecs;
using FluentResults;

namespace EdgeGateway.Data;


/// <summary>
/// 动态类型描述器 适用于自定义属性的类型
/// </summary>
public sealed class DynamicTypeDescriptor : ICodec<DynamicType>
{
    public string Name => throw new NotImplementedException();
    public int? ByteLength => throw new NotImplementedException();


    public Result<DynamicType> Decode(ReadOnlySpan<byte> rawData)
    {
        throw new NotImplementedException();
    }
    public Result<int> Encode(DynamicType value, Span<byte> buffer)
    {
        throw new NotImplementedException();
    }
}
