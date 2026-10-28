using EdgeGateway.Tag;

namespace EdgeGateway.Device;

public interface ITagWriter
{
    ValueTask WriteAsync<T>(ITag<T> tag, T value, CancellationToken cancellationToken = default)
        where T : notnull;
}
