using Microsoft.Extensions.Logging;

namespace EdgeGateway.Logger;


internal sealed class ConsoleLoggerFactory : ILoggerFactory
{
    private LogLevel _minLevel;
    private ConsoleLoggerFactory(LogLevel minLevel) => _minLevel = minLevel;

    public static ConsoleLoggerFactory Trace { get; } = new(LogLevel.Trace);
    public static ConsoleLoggerFactory Debug { get; } = new(LogLevel.Debug);
    public static ConsoleLoggerFactory Information { get; } = new(LogLevel.Information);
    public static ConsoleLoggerFactory Warning { get; } = new(LogLevel.Warning);
    public static ConsoleLoggerFactory Critical { get; } = new(LogLevel.Critical);

    public static ConsoleLoggerFactory FromLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => Trace,
            LogLevel.Debug => Debug,
            LogLevel.Information => Information,
            LogLevel.Warning => Warning,
            LogLevel.Critical => Critical,
            _ => Information
        };
    }


    public ILogger CreateLogger(string categoryName) => new ConsoleLogger(categoryName, _minLevel);
    public void AddProvider(ILoggerProvider provider) { }
    public void Dispose() { }
}