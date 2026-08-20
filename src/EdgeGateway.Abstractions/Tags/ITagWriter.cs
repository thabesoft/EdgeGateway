using FluentResults;

namespace EdgeGateway.Tags;

/// <summary>
/// 提供标签写入能力
/// </summary>
public interface ITagWriter
{
    ValueTask<Result> WriteAsync<T>(Tag<T> tag, T value, CancellationToken cancellationToken = default);
}
