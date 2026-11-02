using Microsoft.Extensions.Logging;

namespace ThabeSoft.EdgeGateway.App.Shell;


public partial class MainViewModel(
        IServiceProvider services,
        ILogger<MainViewModel> logger
    ) : ViewModelBase
{
}