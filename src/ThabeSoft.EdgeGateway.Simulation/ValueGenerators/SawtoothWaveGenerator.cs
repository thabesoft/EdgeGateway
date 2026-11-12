namespace ThabeSoft.EdgeGateway.ValueGenerators;

/// <summary>
/// 锯齿波（Sawtooth Wave）从低线性升到高，然后瞬间回低。
/// </summary>
internal sealed class SawtoothWaveGenerator(double min, double max, double period) : IValueGenerator<double>
{
    private double _current;

    public void Reset() => _current = 0;
    public double Next()
    {
        var phase = _current % period / period;   // 0~1
        _current++;
        return min + phase * (max - min);
    }
}
