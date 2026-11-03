using Microsoft.Extensions.Logging;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


public partial class MainViewModel : ViewModelBase
{
    private readonly IServiceProvider _services;
    private readonly ILogger<MainViewModel> _logger;


    public MainViewModel(IServiceProvider services, ILogger<MainViewModel> logger)
    {
        _services = services;
        _logger = logger;


        Menus =
        [
            NavigationMenuItem.Create<DashboardPageViewModel>("主页"),
            NavigationMenuItem.Create<ManageViewModel>("端口设置"),
        ];

        SelectedMenu = Menus.FirstOrDefault();
    }
}