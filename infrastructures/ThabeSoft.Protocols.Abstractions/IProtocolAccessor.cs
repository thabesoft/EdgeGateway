namespace ThabeSoft.Protocols;


/// <summary>
/// 协议访问器
/// </summary>
public interface IProtocolAccessor : IProtocolReader, IProtocolWriter;


/// <summary>
/// 协议读取器
/// </summary>
public interface IProtocolReader
{
    ValueTask<T> ReadAsync<T>(DataAccessDescriptor<T> tag, CancellationToken cancellationToken = default)
        where T : notnull;
}

/// <summary>
/// 协议写入器
/// </summary>
public interface IProtocolWriter
{
    ValueTask WriteAsync<T>(DataAccessDescriptor<T> tag, T value, CancellationToken cancellationToken = default)
        where T : notnull;
}