using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI.Primitives.Core;
using ScottPlot.DataSources;
using ScottPlot.Plottables;
using System.Runtime.InteropServices;
using ThabeSoft.EdgeGateway.App.Features.Gateway;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;

public partial class SubscriptionChartView : UserControl
{
    private Signal? _signal;
    private SubscriptionViewModel? _vm;


    public SubscriptionChartView()
    {
        InitializeComponent();


        AttachedToVisualTree += (_, _) => ChartRefreshScheduler.Register(this);
        DetachedFromVisualTree += (_, _) => ChartRefreshScheduler.Unregister(this);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        // 行虚拟化会复用控件，DataContext 变化时重建 Signal
        _vm = DataContext as SubscriptionViewModel;
        _signal = null;

        if (_vm is not null)
        {
            //_signal = Plot.Plot.Add.Signal([]);

            //Plot.Plot.Axes.Frameless();
            //Plot.Plot.HideGrid();
            //Plot.Plot.Axes.AutoScale();

            //Plot.Plot.FigureBackground.Color = ScottPlot.Colors.Transparent;
            //Plot.Plot.DataBackground.Color = ScottPlot.Colors.Transparent;
        }
    }

    /// <summary>由页面级定时器调用，读 Buffer 快照并刷新波形。</summary>
    public void RefreshPlot()
    {
        //if (_vm is null || _signal is null) return;

        //var data = _vm.Buffer.Snapshot();
        //if (data.Length < 2) return;

        //_signal.Data = new SignalSourceDouble(data, 1);
        //Plot.Plot.Axes.SetLimitsX(0, data.Length);
        //Plot.Refresh();


        Sparkline.InvalidateVisual();
    }
}

internal static class ChartRefreshScheduler
{
    private static readonly DispatcherTimer Timer = new()
    {
        Interval = TimeSpan.FromMilliseconds(100)
    };

    private static readonly HashSet<SubscriptionChartView> Charts = new();

    static ChartRefreshScheduler()
    {
        Timer.Tick += (_, _) =>
        {
            foreach (var chart in Charts)
                chart.RefreshPlot();
        };
        Timer.Start();
    }

    public static void Register(SubscriptionChartView chart) => Charts.Add(chart);

    public static void Unregister(SubscriptionChartView chart) => Charts.Remove(chart);
}