namespace ThabeSoft.EdgeGateway.ValueGenerators;

/// <summary>
/// 随机值（Random）在范围内均匀随机，适合模拟噪声。
/// </summary>
internal sealed class RandomValueGenerator(double min, double max, int? seed = null) : IValueGenerator<double>
{
    private readonly Random _random = seed.HasValue ? new Random(seed.Value) : new Random();

    public void Reset() { }
    public double Next() => min + (_random.NextDouble() * (max - min));
}
