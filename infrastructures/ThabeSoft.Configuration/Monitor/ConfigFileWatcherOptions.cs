namespace ThabeSoft.Configuration.Monitor;


public record class ConfigFileWatcherOptions
{
    public string FilePath { get; set; } = "Config.json";
    public TimeSpan DebouncePeriod { get; set; } = TimeSpan.FromSeconds(3);
}