using EdgeGateway.Tags;

namespace EdgeGateway.Devices;

public interface ITagReader
{
    ValueTask<T> ReadAsync<T>(ITag<T> tag, CancellationToken cancellationToken = default);
}
