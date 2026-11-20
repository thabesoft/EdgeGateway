namespace ThabeSoft.Configuration.Rloader;


public record class ConfigReloadOptions
{
    public string FilePath { get; set; } = "Config.json";
    public TimeSpan DebouncePeriod { get; set; } = TimeSpan.FromSeconds(3);
}