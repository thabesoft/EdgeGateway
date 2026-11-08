namespace ThabeSoft.EdgeGateway.Simulation.ValueGenerators;


public interface IValueGenerator<T>
    where T : notnull
{
    void Reset();
    T Next();
}


public sealed class ValueGenerator
{
    /// <summary>
    /// 正弦波
    /// </summary>
    /// <param name="amplitude">振幅</param>
    /// <param name="step">相位增量</param>
    /// <returns></returns>
    public static IValueGenerator<double> SineWave(double amplitude, double step) =>
        new SineWaveGenerator(amplitude, step);
}