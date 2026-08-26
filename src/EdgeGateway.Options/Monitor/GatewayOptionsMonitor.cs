using EdgeGateway.Options.Loader;
using EdgeGateway.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace EdgeGateway.Options.Monitor;


internal sealed partial class GatewayOptionsMonitor : IGatewayOptionsMonitor
{
    private readonly FileSystemWatcher _watcher;
    private readonly IGatewayOptionsLoader _loader;
    private readonly ILogger<GatewayOptionsMonitor> _logger;
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    private CancellationTokenSource _loadCts = new();


    public GatewayOptionsMonitor(
        IOptions<GatewaySourceOptions> options,
        IGatewayOptionsLoader loader,
        ILogger<GatewayOptionsMonitor> logger)
    {
        var optionsFilePath = options.Value.Path;

        _loader = loader;
        _logger = logger;

        var optionFileDirectory = Path.GetDirectoryName(optionsFilePath);
        if (string.IsNullOrWhiteSpace(optionFileDirectory)) optionFileDirectory = AppContext.BaseDirectory;

        _watcher = new(optionFileDirectory, Path.GetFileName(optionsFilePath))
        {
            NotifyFilter = NotifyFilters.LastWrite
        };

        _watcher.Changed += OnOptionsFileChanged;
        _watcher.EnableRaisingEvents = true;
    }

    private void OnOptionsFileChanged(object sender, FileSystemEventArgs e)
    {
        Task.Run(async () =>
        {
            try
            {
                await _loadLock.WaitAsync(_loadCts.Token);
                if (_loadCts.IsCancellationRequested) return;

#if NETSTANDARD2_0
                    _loadCts.Cancel();
#else
                await _loadCts.CancelAsync();
#endif

                _loadCts = new();

                var result = await _loader.LoadAsync(_loadCts.Token);
                if (result.IsSuccess)
                {
                    Publish(result.Value);
                    return;
                }

                _logger.LogError("配置加载失败 {msg}", result.ToString());
            }
            catch(OperationCanceledException ex)
            {
                _logger.LogDebug(ex, "加载任务已被取消, 正在重新加载");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "配置加载失败");
            }
            finally
            {
                _loadLock.Release();
            }

        }, _loadCts.Token);
    }
}


internal sealed partial class GatewayOptionsMonitor
{
    public GatewayOptions? CurrentOptions { get; private set; }


    #region --Events--

    // 添加端口
    public Subject<IPortOptions> PortAdded { get; } = new();
    IObservable<IPortOptions> IGatewayOptionsMonitor.PortAdded => PortAdded;

    // 删除端口
    public Subject<IPortOptions> PortRemoved { get; } = new();
    IObservable<IPortOptions> IGatewayOptionsMonitor.PortRemoved => PortRemoved;

    #endregion


    public void Publish(GatewayOptions gatewayOptions)
    {
        if (CurrentOptions == gatewayOptions) return;

        PublishPortChanges(CurrentOptions?.Ports ?? [], gatewayOptions.Ports);


        CurrentOptions = gatewayOptions;
    }


    private void PublishPortChanges(IEnumerable<IPortOptions> old, IEnumerable<IPortOptions> @new)
    {
        var oldPorts = old.ToDictionary(k => k.Address, v => v);

        foreach (var newPort in @new.ToArray())
        {
            // 新增 (旧集合中不存在)
            if (!oldPorts.TryGetValue(newPort.Address, out var _))
            {
                PortAdded.OnNext(newPort);
            }
            // 不变 (旧集合中存在)
            else
            {
                oldPorts.Remove(newPort.Address);
            }
        }

        // 新老集合中都不存在
        foreach (var port in oldPorts.Values.ToArray())
        {
            PortRemoved.OnNext(port);
        }
    }
}