using CommunityToolkit.Mvvm.ComponentModel;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.App.ViewModels;

internal sealed partial class TagViewModel(ITag monitor) : ViewModelBase
{
    [ObservableProperty]
    public partial string Name { get; set; } = monitor.Id.ToString();

    [ObservableProperty]
    public partial string Address { get; set; } = monitor.Address.ToString() ?? "";


    [ObservableProperty]
    public partial TagQuality State { get; set; } = monitor.Quality;

    [ObservableProperty]
    public partial object? Value { get; set; } = monitor.Value;

    [ObservableProperty]
    public partial DateTimeOffset UpdateTime { get; set; } = monitor.UpdateTime;
}