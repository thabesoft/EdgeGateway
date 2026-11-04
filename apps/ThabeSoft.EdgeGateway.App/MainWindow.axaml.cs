using SukiUI.Controls;
using ThabeSoft.EdgeGateway.App.Designs;

namespace ThabeSoft.EdgeGateway.App;

public partial class MainWindow : SukiWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DesignHost.SetDataContext<MainWindowViewModel>(this);
    }
}
