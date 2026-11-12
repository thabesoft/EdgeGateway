using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Devices.Modbus;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.ValueGenerators;

namespace ThabeSoft.EdgeGateway;

public static class Simulation
{
    public static SimulatorPortDescriptor ModbusRtuPort(SerialTransportDescriptor transportDescriptor) =>
             new(transportDescriptor, Modbus.Rtu);
    public static SimulatorPortDescriptor ModbusAsciiPort(SerialTransportDescriptor transportDescriptor) =>
        new(transportDescriptor, Modbus.Ascii);
    public static SimulatorPortDescriptor ModbusIpPort(IpTransportDescriptor transportDescriptor) =>
        new(transportDescriptor, Modbus.Ip);



    public static SimulationDeviceDescriptor ModbusDevice(byte unitId) =>
            new(new ModbusSimulationDeviceId(unitId));


    #region --值生成器--

    /// <summary>正弦波生成器。</summary>
    /// <param name="amplitude">振幅（波峰相对中心值的偏移量）。</param>
    /// <param name="step">相位增量（每次 <see cref="IValueGenerator{T}.Next"/> 时相位增加的量）。</param>
    public static IValueGenerator<double> SineWaveValue(double amplitude, double step)
        => new SineWaveGenerator(amplitude, step);

    /// <summary>方波生成器，在 <paramref name="low"/> 和 <paramref name="high"/> 之间切换。</summary>
    /// <param name="low">低电平值。</param>
    /// <param name="high">高电平值。</param>
    /// <param name="period">周期（每多少个采样点为一次完整循环）。</param>
    /// <param name="duty">占空比（高电平在一个周期内的占比，0~1，默认 0.5）。</param>
    public static IValueGenerator<double> SquareWaveValue(double low, double high, double period, double duty = 0.5)
        => new SquareWaveGenerator(low, high, period, duty);

    /// <summary>三角波生成器，在 <paramref name="min"/> 和 <paramref name="max"/> 之间线性往复。</summary>
    /// <param name="min">谷值（最小值）。</param>
    /// <param name="max">峰值（最大值）。</param>
    /// <param name="period">周期（每多少个采样点为一次完整循环）。</param>
    public static IValueGenerator<double> TriangleWaveValue(double min, double max, double period)
        => new TriangleWaveGenerator(min, max, period);

    /// <summary>锯齿波生成器，从 <paramref name="min"/> 线性升到 <paramref name="max"/> 后瞬间回落到 <paramref name="min"/>。</summary>
    /// <param name="min">起始值（最小值）。</param>
    /// <param name="max">终点值（最大值）。</param>
    /// <param name="period">周期（每多少个采样点为一次完整循环）。</param>
    public static IValueGenerator<double> SawtoothWaveValue(double min, double max, double period)
        => new SawtoothWaveGenerator(min, max, period);

    /// <summary>随机值生成器，在 <paramref name="min"/> 和 <paramref name="max"/> 之间均匀分布。</summary>
    /// <param name="min">最小值（含）。</param>
    /// <param name="max">最大值（不含）。</param>
    /// <param name="seed">随机种子；传 null 使用时间种子（不可重现），传固定值可重现序列。</param>
    public static IValueGenerator<double> RandomValue(double min, double max, int? seed = null)
        => new RandomValueGenerator(min, max, seed);

    /// <summary>阶梯生成器，从 <paramref name="start"/> 开始每次增加 <paramref name="step"/>，超过 <paramref name="max"/> 后回绕。</summary>
    /// <param name="start">起始值。</param>
    /// <param name="step">每次递增的步长。</param>
    /// <param name="max">上限；超过后回绕到 <paramref name="start"/>。</param>
    public static IValueGenerator<double> StepValue(double start, double step, double max)
        => new StepGenerator(start, step, max);

    /// <summary>常量生成器，每次都返回同一个值。</summary>
    /// <param name="value">固定输出值。</param>
    public static IValueGenerator<double> ConstantValue(double value)
        => new ConstantGenerator(value);

    /// <summary>带噪声的正弦波生成器，在正弦波基础上叠加随机抖动，模拟真实传感器读数。</summary>
    /// <param name="amplitude">正弦波振幅。</param>
    /// <param name="offset">正弦波中心值（直流偏置）。</param>
    /// <param name="period">正弦波周期（每多少个采样点为一次完整循环）。</param>
    /// <param name="noise">噪声幅度（叠加的随机抖动的最大偏移）。</param>
    public static IValueGenerator<double> NoisySineWaveValue(double amplitude, double offset, double period, double noise)
        => new NoisySineWaveGenerator(amplitude, offset, period, noise);


    #endregion
}
