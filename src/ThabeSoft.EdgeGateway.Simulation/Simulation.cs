using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.ValueGenerators;

namespace ThabeSoft.EdgeGateway;

public static class Simulation
{
    public sealed class Networks
    {
        public static SimulatorPortDescriptor ModbusRtu(SerialTransportDescriptor transportDescriptor) =>
            new(transportDescriptor, Modbus.Rtu);
        public static SimulatorPortDescriptor ModbusAscii(SerialTransportDescriptor transportDescriptor) =>
            new(transportDescriptor, Modbus.Ascii);
        public static SimulatorPortDescriptor ModbusIp(IpTransportDescriptor transportDescriptor) =>
            new(transportDescriptor, Modbus.Ip);
    }


    /// <summary>
    /// 值生成器
    /// </summary>
    public static class Values
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
}
