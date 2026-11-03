using Avalonia.Controls;
using System.Diagnostics;
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

    private void OnPointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        Debug.WriteLine("ManageView_ListView_OnPointerPressed");
    }
}