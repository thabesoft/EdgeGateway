using EdgeGateway.Tag;

namespace EdgeGateway.Device;

public interface ITagReader
{
    ValueTask<T> ReadAsync<T>(ITag<T> tag, CancellationToken cancellationToken = default)
        where T : notnull;
}