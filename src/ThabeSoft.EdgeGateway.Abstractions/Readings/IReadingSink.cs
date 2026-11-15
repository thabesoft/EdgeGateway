namespace ThabeSoft.EdgeGateway.Readings;


/// <summary>
/// 数据接收端
/// </summary>
public interface IReadingSink
{
    ValueTask WriteAsync<T>(Reading<T> reading, CancellationToken cancellationToken = default) where T : notnull;
}
