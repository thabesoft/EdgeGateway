namespace ThabeSoft.EdgeGateway.ValueGenerators;


/// <summary>
/// 正弦波
/// </summary>
internal sealed class SineWaveGenerator : IValueGenerator<double>
{
    private double _phase;
    private readonly double amplitude;
    private readonly double step;

    public SineWaveGenerator(double amplitude, double step)
    {
        ArgumentOutOfRangeException.ThrowIfZero(amplitude);

        this.amplitude = amplitude;
        this.step = step;
    }

    public void Reset()
    {
        _phase = 0;
    }
    public double Next()
    {
        _phase += step;
        return (Math.Sin(_phase) * amplitude) + amplitude;
    }
}