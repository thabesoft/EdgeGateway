using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


internal delegate DeviceViewModel DeviceViewModelFactory(IDevice device);

internal sealed partial class DeviceViewModel(IDevice device) : ViewModelBase
{
    public DeviceId Id => device.Id;
    public ProtocolType ProtocolType => device.ProtocolType;


    [ObservableProperty]
    public partial bool DisplayName { get; set; }

    [ObservableProperty]
    public partial bool IsEnabled { get; set; } = device.IsEnabled;


    [RelayCommand]
    private async Task EnableAsync()
    {
        await device.EnableAsync();
        IsEnabled = device.IsEnabled;
    }

    [RelayCommand]
    private async Task DisableAsync()
    {
        await device.DisableAsync();
        IsEnabled = device.IsEnabled;
    }
}