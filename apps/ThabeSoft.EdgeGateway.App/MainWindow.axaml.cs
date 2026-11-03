using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using SukiUI.Controls;
using SukiUI.Enums;
using SukiUI.Models;
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
