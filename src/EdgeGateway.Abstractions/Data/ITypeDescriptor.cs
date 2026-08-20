using FluentResults;

namespace EdgeGateway.Data;


/// <summary>
/// 数据反序列化接口
/// </summary>
public interface ITypeDescriptor<T>
{
    /// <summary>
    /// 类型的名称（如 "Int32", "DateTime", "StatusWord"）
    /// </summary>
    string Name { get; }
    int? ByteLength { get; }


    Result<T> Decode(ReadOnlySpan<byte> rawData);
    Result<int> Encode(T value, Span<byte> buffer);
}