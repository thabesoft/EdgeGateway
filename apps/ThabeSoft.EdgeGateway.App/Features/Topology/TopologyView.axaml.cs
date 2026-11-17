using Avalonia.Controls;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;

public partial class TopologyView : UserControl
{
    public TopologyView()
    {
        InitializeComponent();
        Initialized += OnInitialized;
    }

    private void OnInitialized(object? sender, EventArgs e)
    {
        if (DataContext is not TopologyViewModel vm) return;
        vm.OnNavigationEnter();
    }
}