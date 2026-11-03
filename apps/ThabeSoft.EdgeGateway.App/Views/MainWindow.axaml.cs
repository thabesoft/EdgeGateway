using Avalonia.Controls;
using ThabeSoft.EdgeGateway.App.Designs;
using ThabeSoft.EdgeGateway.App.ViewModels;

namespace ThabeSoft.EdgeGateway.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DesignHost.SetDataContext<MainWindowViewModel>(this);
    }
}
