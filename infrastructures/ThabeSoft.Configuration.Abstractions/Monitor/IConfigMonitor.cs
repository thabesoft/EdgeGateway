namespace ThabeSoft.Configuration.Monitor;


public interface IConfigMonitor<T>
{
    T? CurrentValue { get; }
    IDisposable OnChange(Func<T, CancellationToken, ValueTask> action);
}
