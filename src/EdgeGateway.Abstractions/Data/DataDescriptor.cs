using EdgeGateway.Codecs;

namespace EdgeGateway.Data;

public interface IDataDescriptor
{
    IDataAddress Address { get; }
}

/// <summary>
/// 数据描述符，包含数据的地址和类型描述符
/// </summary>
/// <typeparam name="T">数据在C#中的类型</typeparam>
public record class DataDescriptor<T> : IDataDescriptor
    where T : unmanaged
{
    public required IDataAddress Address { get; init; }
    public required ICodec<T> TypeDescriptor { get; init; }


    public static DataDescriptor<T> Byte(IDataAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);

        return new DataDescriptor<T>() { Address = address, TypeDescriptor = typeDescriptor };
    }
}