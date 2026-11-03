using Avalonia.Controls;
using ThabeSoft.EdgeGateway.App.Designs;
using ThabeSoft.EdgeGateway.App.ViewModels;

namespace ThabeSoft.EdgeGateway.App.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DesignHost.SetDataContext<MainViewModel>(this);
    }
}
