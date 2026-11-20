namespace ThabeSoft.Protocols;

public interface IProtocolReader
{
    ValueTask<T> ReadAsync<T>(DataAccessDescriptor<T> tag, CancellationToken cancellationToken = default)
        where T : notnull;
}