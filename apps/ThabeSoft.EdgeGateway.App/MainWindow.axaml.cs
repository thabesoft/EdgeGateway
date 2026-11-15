using SukiUI.Controls;
using ThabeSoft.EdgeGateway.App.Hosting;

namespace ThabeSoft.EdgeGateway.App;

public partial class MainWindow : SukiWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DesignHost.SetDataContext<MainWindowViewModel>(this);
    }
}
