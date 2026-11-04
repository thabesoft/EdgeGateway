using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


internal delegate DeviceViewModel DeviceViewModelFactory(IDevice device);

internal sealed partial class DeviceViewModel(IDevice device) : ViewModelBase
{
    private readonly AvaloniaList<TagViewModel> _tagMonitors = [..device.TagMonitors.Values.Select(device =>
        new TagViewModel(device))];


    public DeviceId Id => device.Id;
    public ProtocolType ProtocolType => device.ProtocolType;
    public ICollection<TagViewModel> TagMonitors => _tagMonitors;



    [ObservableProperty]
    public partial bool DisplayName { get; set; }

    [ObservableProperty]
    public partial bool IsEnabled { get; set; } = device.IsEnabled;


    [RelayCommand]
    private async Task EnableAsync()
    {
        await device.EnableAsync();
        RefreshIsEnabled();
    }

    [RelayCommand]
    private async Task DisableAsync()
    {
        await device.DisableAsync();
        RefreshIsEnabled();
    }


    public void RefreshIsEnabled() => IsEnabled = device.IsEnabled;
}