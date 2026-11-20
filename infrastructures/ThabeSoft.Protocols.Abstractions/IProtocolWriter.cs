namespace ThabeSoft.Protocols;

public interface IProtocolWriter
{
    ValueTask WriteAsync<T>(DataAccessDescriptor<T> tag, T value, CancellationToken cancellationToken = default)
        where T : notnull;
}