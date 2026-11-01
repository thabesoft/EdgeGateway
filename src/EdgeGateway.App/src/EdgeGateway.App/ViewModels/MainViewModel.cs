using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace EdgeGateway.App.ViewModels;


public partial class MainViewModel(
        IServiceProvider services,
        ILogger<MainViewModel> logger
    ) : ViewModelBase
{
}