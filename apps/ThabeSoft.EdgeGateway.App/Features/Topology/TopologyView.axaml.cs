using Avalonia.Controls;
using ThabeSoft.EdgeGateway.App.Designs;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;

public partial class TopologyView : UserControl
{
    public TopologyView()
    {
        InitializeComponent();
        this.Initialized += OnInitialized;

        DesignHost.SetDataContext<TopologyViewModel>(this);
    }

    private void OnInitialized(object? sender, EventArgs e)
    {
        if (DataContext is not TopologyViewModel vm) return;
        vm.OnNavigationEnter();
    }
}