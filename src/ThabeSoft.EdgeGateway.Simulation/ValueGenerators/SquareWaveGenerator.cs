namespace ThabeSoft.EdgeGateway.ValueGenerators;


/// <summary>
/// 方波（Square Wave）在高低两个电平间切换，带占空比。
/// </summary>
internal sealed class SquareWaveGenerator(double low, double high, double period, double dutyCycle = 0.5) : IValueGenerator<double>
{
    private double _current;
    private readonly double _dutyCycle = Math.Clamp(dutyCycle, 0, 1);   // 0~1，高电平占比


    public void Reset() => _current = 0;
    public double Next()
    {
        double phase = (_current % period) / period;   // 0~1
        _current++;
        return phase < _dutyCycle ? high : low;
    }
}
