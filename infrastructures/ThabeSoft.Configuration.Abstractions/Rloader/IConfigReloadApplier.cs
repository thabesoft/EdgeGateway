namespace ThabeSoft.Configuration.Rloader;


/// <summary>
/// 配置重载应用器
/// </summary>
public interface IConfigReloadApplier
{
    Task ApplyAsync(object newConfig, CancellationToken cancellationToken = default);
}