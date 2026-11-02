using CommunityToolkit.Mvvm.ComponentModel;
using ThabeSoft.EdgeGateway.Channels;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


internal sealed partial class IpPortFormViewModel : ViewModelBase
{
    public IpConnectionType[] ConnectionTypeOptions { get; } = [IpConnectionType.Tcp, IpConnectionType.Udp];


    [ObservableProperty]
    public partial string Address { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int Port { get; set; } = 0;

    [ObservableProperty]
    public partial IpConnectionType ConnectionType { get; set; } = IpConnectionType.Tcp;
}


internal sealed partial class IpChannelDescriptorViewModel : ObservableValidator
{
    private readonly IpChannelDescriptorBuilder builder = new();


    [ObservableProperty]
    public partial string Address { get; set; } = string.Empty;



    partial void OnAddressChanged(string value)
    {
        builder.Address(Address);
    }


    public IpChannelDescriptor Build()
    {
        return builder.Build();
    }
}