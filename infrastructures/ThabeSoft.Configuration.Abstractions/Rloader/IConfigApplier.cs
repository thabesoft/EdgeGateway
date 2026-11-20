namespace ThabeSoft.Configuration.Rloader;


public interface IConfigApplier
{
    Task ApplyAsync(object newConfig, CancellationToken cancellationToken = default);
}