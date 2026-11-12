namespace ThabeSoft.EdgeGateway.ValueGenerators;

/// <summary>
/// 正弦波 + 噪声（Sine + Noise）
/// </summary>
internal sealed class NoisySineWaveGenerator : IValueGenerator<double>
{
    private readonly double _amplitude;
    private readonly double _offset;
    private readonly double _period;
    private readonly double _noiseAmplitude;
    private readonly Random _random = new();
    private double _t;

    public NoisySineWaveGenerator(double amplitude, double offset, double period, double noiseAmplitude)
    {
        _amplitude = amplitude;
        _offset = offset;
        _period = period;
        _noiseAmplitude = noiseAmplitude;
    }

    public void Reset() => _t = 0;

    public double Next()
    {
        var sine = _offset + _amplitude * Math.Sin(2 * Math.PI * _t / _period);
        var noise = (_random.NextDouble() - 0.5) * 2 * _noiseAmplitude;
        _t += 1;
        return sine + noise;
    }
}