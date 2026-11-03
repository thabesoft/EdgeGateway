using Avalonia.Controls;
using ThabeSoft.EdgeGateway.App.Designs;
using ThabeSoft.EdgeGateway.App.ViewModels;

namespace ThabeSoft.EdgeGateway.App.Views;

public partial class ManageView : UserControl
{
    public ManageView()
    {
        InitializeComponent();
        DesignHost.SetDataContext<ManageViewModel>(this);
    }
}