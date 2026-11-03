using Avalonia.Controls;
using ThabeSoft.EdgeGateway.App.Designs;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;

public partial class TopologyView : UserControl
{
    public TopologyView()
    {
        InitializeComponent();
        DesignHost.SetDataContext<TopologyViewModel>(this);
    }
}