//using ThabeSoft.EdgeGateway.Devices;

//#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
//namespace ThabeSoft.EdgeGateway.Ports;
//#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


//public static class ModbusDeviceExtensions
//{
//    extension(IPort port)
//    {
//        public ValueTask<IDevice> AddModbusRtuDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default) =>
//            port.AddDeviceAsync(deviceId, Modbus.Rtu, cancellationToken);
//        public ValueTask<IDevice> AddModbusRtuDeviceAsync(CancellationToken cancellationToken = default) =>
//            port.AddModbusRtuDeviceAsync(DeviceId.Create(), cancellationToken);


//        public ValueTask<IDevice> AddModbusAsciiDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default) =>
//           port.AddDeviceAsync(deviceId, Modbus.Ascii, cancellationToken);
//        public ValueTask<IDevice> AddModbusAsciiDeviceAsync(CancellationToken cancellationToken = default) =>
//            port.AddModbusAsciiDeviceAsync(DeviceId.Create(), cancellationToken);


//        public ValueTask<IDevice> AddModbusIpDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default) =>
//           port.AddDeviceAsync(deviceId, Modbus.Ip, cancellationToken);
//        public ValueTask<IDevice> AddModbusIpDeviceAsync(CancellationToken cancellationToken = default) =>
//            port.AddModbusIpDeviceAsync(DeviceId.Create(), cancellationToken);
//    }
//}
