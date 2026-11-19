namespace ThabeSoft.EdgeGateway.Configuration.Rloader;


public record class ConfigReloadOptions
{
    public const string SectionName = "Config";

    public string FilePath { get; set; } = "Gateway.json";
    public TimeSpan DebouncePeriod { get; set; } = TimeSpan.FromSeconds(3);
}