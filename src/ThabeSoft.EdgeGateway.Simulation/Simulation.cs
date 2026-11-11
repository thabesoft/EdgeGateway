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



    /// <summary>
    /// 正弦波
    /// </summary>
    /// <param name="amplitude">振幅</param>
    /// <param name="step">相位增量</param>
    /// <returns></returns>
    public static IValueGenerator<double> SineWaveValue(double amplitude, double step) =>
        new SineWaveGenerator(amplitude, step);
}
