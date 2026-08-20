using Microsoft.Extensions.Logging;

namespace EdgeGateway.Logger;

internal sealed class ConsoleLogger(string categoryName, LogLevel minLevel) : ILogger
{
    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
    {
        // 不需要 Scope，返回空实现
        return NullDisposable.Instance;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= minLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string?> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var originalColor = Console.ForegroundColor;
        try
        {
            // 设置颜色
            Console.ForegroundColor = logLevel switch
            {
                LogLevel.Critical => ConsoleColor.Red,
                LogLevel.Error => ConsoleColor.DarkRed,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Information => ConsoleColor.Cyan,
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Trace => ConsoleColor.DarkGray,
                _ => ConsoleColor.White
            };

            // 格式化消息
            var message = formatter(state, exception);
            var time = DateTime.Now.ToString("HH:mm:ss.fff");

            // 输出
            Console.WriteLine($"[{logLevel}] [{time}] [{logLevel}] [{categoryName}] {message}");

            // 如果有异常，单独输出
            if (exception is not null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  Exception: {exception.Message}");
                Console.WriteLine($"  StackTrace: {exception.StackTrace}");
            }
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }
    }

    private sealed class NullDisposable : IDisposable
    {
        public static NullDisposable Instance { get; } = new NullDisposable();
        public void Dispose() { }
    }
}
