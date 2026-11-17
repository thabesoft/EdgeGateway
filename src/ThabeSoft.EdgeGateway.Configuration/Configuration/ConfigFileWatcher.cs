using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ThabeSoft.EdgeGateway.Configuration;


public sealed class ConfigFileWatcher : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    private readonly TimeSpan _debounce;
    private readonly ILogger<ConfigFileWatcher> _logger;
    private readonly SemaphoreSlim _debounceLock = new(1, 1);
    private CancellationTokenSource? _debounceCts;


    public event Func<CancellationToken, Task>? Changed;


    public ConfigFileWatcher(string filePath, TimeSpan? debounce = null, ILogger<ConfigFileWatcher>? logger = null)
    {
        var fullPath = Path.GetFullPath(filePath);
        var dir = Path.GetDirectoryName(fullPath)!;
        var fileName = Path.GetFileName(fullPath);

        _debounce = debounce ?? TimeSpan.FromMilliseconds(500);
        _logger = logger ?? NullLogger<ConfigFileWatcher>.Instance;

        _watcher = new FileSystemWatcher(dir, fileName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
            EnableRaisingEvents = true
        };

        // 用同一个处理器，统一防抖
        _watcher.Changed += OnFileChanged;
        _watcher.Created += OnFileChanged;
        _watcher.Renamed += OnFileChanged;
        
        FilePath = fullPath;
    }

    public string FilePath { get;  }


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
                    await Task.Delay(_debounce, cts.Token);
                    if (Changed is { } handler)
                        await handler(cts.Token);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex) { _logger.LogError(ex, "配置变化回调失败"); }
            }, cts.Token);
        }
        finally { _debounceLock.Release(); }
    }

    public void Dispose()
    {
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _debounceLock.Dispose();
    }
}
