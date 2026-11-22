using Microsoft.Extensions.Logging;
using ThabeSoft.Primitives;

namespace ThabeSoft.Configuration.Monitor;


internal sealed class ConfigFileWatcher : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    private readonly ILogger<ConfigFileWatcher> _logger;

    private readonly SemaphoreSlim _debounceLock = new(1, 1);
    private CancellationTokenSource? _debounceCts;


    public ConfigFileWatcher(string filePath, ILogger<ConfigFileWatcher> logger)
    {
        var fullPath = Path.GetFullPath(filePath);
        var dir = Path.GetDirectoryName(fullPath);
        var fileName = Path.GetFileName(fullPath);

        FilePath = fullPath;
        _logger = logger;

        _watcher = new FileSystemWatcher(dir, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
            EnableRaisingEvents = true
        };
        _watcher.Changed += OnFileChanged;
        _watcher.Created += OnFileChanged;
        _watcher.Renamed += OnFileChanged;
    }


    public event Func<CancellationToken, Task>? Changed;
    public TimeSpan Debounce { get; init; } = TimeSpan.FromMilliseconds(100);
    public string FilePath { get; }


    public void Dispose()
    {
        _watcher.EnableRaisingEvents = false;
        _watcher?.DisposeSafely(_logger);

        _debounceCts?.Cancel();
        _debounceCts?.DisposeSafely(_logger);

        _debounceLock.DisposeSafely(_logger);
    }


    private async void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        // 防抖：取消上一次等待，重新计时
        await _debounceLock.WaitAsync();
        try
        {
            _debounceCts?.Cancel();
            _debounceCts = new CancellationTokenSource();
            var cts = _debounceCts;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(Debounce, cts.Token);
                    if (Changed is { } handler)
                        await handler(cts.Token);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex) { _logger.LogError(ex, "配置变化回调失败"); }
            }, cts.Token);
        }
        finally { _debounceLock.Release(); }
    }

}