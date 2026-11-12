namespace ThabeSoft.EdgeGateway.ValueGenerators;

/// <summary>
/// 阶梯递增（Counter / Ramp）每次增加固定步长，到上限后回绕。适合模拟累计量。
/// </summary>
internal sealed class StepGenerator(double start, double step, double max) : IValueGenerator<double>
{
    private double _current;
    public void Reset() => _current = start;

    public double Next()
    {
        _current += step;
        if (_current > max) _current = start;   // 回绕
        return _current;
    }
}