namespace ThabeSoft.Simulator.ValueGenerators;

/// <summary>
/// 正弦波
/// </summary>
public sealed class SineWaveGenerator(double amplitude, double step) : IValueGenerator<double>
{
    private double _phase;

    public void Reset()
    {
        _phase = 0;
    }
    public void Next(Action<double> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        _phase += step;
        var value = (Math.Sin(_phase) * amplitude) + amplitude;
        action.Invoke(value);
    }
}