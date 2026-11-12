namespace ThabeSoft.EdgeGateway.ValueGenerators;

/// <summary>
/// 三角波（Triangle Wave）
/// 从低到高再到低，线性变化。
/// </summary>
internal sealed class TriangleWaveGenerator(double min, double max, double period) : IValueGenerator<double>
{
    private double _current;

    public void Reset() => _current = 0;
    public double Next()
    {
        var phase = (_current % period) / period;   // 0~1
        _current++;
        // 0→1 上升，1→0 下降
        var normalized = phase < 0.5 ? phase * 2 : 2 - phase * 2;
        return min + normalized * (max - min);
    }
}
