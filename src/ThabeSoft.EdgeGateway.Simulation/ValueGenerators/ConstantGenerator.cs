namespace ThabeSoft.EdgeGateway.ValueGenerators;

/// <summary>
/// 常量 一直返回同一个值，适合固定值测试。
/// </summary>
public sealed class ConstantGenerator(double value) : IValueGenerator<double>
{
    public void Reset() { }
    public double Next() => value;
}