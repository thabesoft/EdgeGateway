using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ThabeSoft.EdgeGateway.App.Features.Gateway;

namespace ThabeSoft.EdgeGateway.App.Controls;

public sealed class SparklineControl : Control
{
    // 定义数据源属性
    public static readonly StyledProperty<RingBuffer<double>?> BufferProperty =
        AvaloniaProperty.Register<SparklineControl, RingBuffer<double>?>(nameof(Buffer));

    public RingBuffer<double>? Buffer
    {
        get => GetValue(BufferProperty);
        set => SetValue(BufferProperty, value);
    }

    static SparklineControl()
    {
        // 当 Buffer 变化时，自动触发重绘（但如果是 Buffer 内部数据变了，需要手动调 InvalidateVisual）
        AffectsRender<SparklineControl>(BufferProperty);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Buffer is null) return;
        // 确保有尺寸
        if (Bounds.Width <= 0 || Bounds.Height <= 0) return;

        // 2. 拿真实数据
        var data = Buffer.Snapshot();

        // 如果数据少于 2 个点，画个红框提醒一下（方便调试）
        if (data.Length < 2)
        {
            context.FillRectangle(Brushes.Red, new Rect(0, 0, Bounds.Width, Bounds.Height));
            return;
        }

        double width = Bounds.Width;
        double height = Bounds.Height;
        double min = data.Min();
        double max = data.Max();
        double range = max - min;

        // 防止除以 0（如果你现在的数据全是 0，range 就是 0）
        if (range == 0) range = 1;

        var pen = new Pen(Brushes.Blue, 1.5);
        var geometry = new StreamGeometry();

        using (var ctx = geometry.Open())
        {
            for (int i = 0; i < data.Length; i++)
            {
                double x = i * (width / (data.Length - 1));
                double normalized = (data[i] - min) / range;
                double y = height - (normalized * height); // Y 轴反转

                var point = new Point(x, y);
                if (i == 0) ctx.BeginFigure(point, false);
                else ctx.LineTo(point);
            }
        }

        context.DrawGeometry(null, pen, geometry);
    }
}