using EdgeGateway.Tags;

namespace EdgeGateway.Devices;

public interface ITagWriter
{
    ValueTask WriteAsync<T>(ITag<T> tag, T value, CancellationToken cancellationToken = default);
}
