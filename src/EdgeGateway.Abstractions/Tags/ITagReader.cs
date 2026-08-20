using FluentResults;

namespace EdgeGateway.Tags;


/// <summary>
/// 提供标签读取能力
/// </summary>
public interface ITagReader
{
    ValueTask<Result<T>> ReadAsync<T>(Tag<T> tag, CancellationToken cancellationToken = default);
}