using EdgeGateway.Devices;
using EdgeGateway.Devices.Modbus;
using EdgeGateway.Options;
using EdgeGateway.Ports.SerialPort;
using EdgeGateway.Protocols;
using EdgeGateway.Tags;
using System.Runtime.CompilerServices;

namespace EdgeGateway;


[TestClass]
public sealed class GatewayTests
{
    //[TestMethod(DisplayName = "完整流程测试")]
    public async Task FullProcess()
    {
        await using var gateway = Gateway.Create();

        // 初始化
        gateway.PortFactory.Register(SerialPortFactory.Instance);
        gateway.DeviceFactory.Register(ModbusDeviceFactory.Instance);

        // 添加端口
        var portConfig = SerialPortOptions.Builder("COM2").Build();
        var addPortResult = gateway.Port.Add(portConfig);
        if (addPortResult.IsFailed) Assert.IsTrue(addPortResult.IsSuccess, addPortResult.ToString());
        // 开启
        var port = addPortResult.Value;
        var openPortResult = await port.OpenAsync(TestContext.CancellationToken);
        if (openPortResult.IsFailed) Assert.IsTrue(openPortResult.IsSuccess, openPortResult.ToString());

        //添加设备
        var deviceConfig = new DeviceOptions(DeviceId.Parse("温度传感器"), ProtocolType.Modbus, portConfig.Address);
        var addDeviceResult = gateway.Device.Add(port, deviceConfig); // port.AddDevice(deviceConfig);
        if (addDeviceResult.IsFailed) Assert.IsTrue(addDeviceResult.IsSuccess, addDeviceResult.ToString());
        // 连接
        var device = addDeviceResult.Value;
        var deviceConnectResult = await device.ConnectAsync(TestContext.CancellationToken);
        if (deviceConnectResult.IsFailed) Assert.IsTrue(deviceConnectResult.IsSuccess, deviceConnectResult.ToString());

        // 标签
        var tag = Tag.Create<int>(portConfig.Address, deviceConfig.DeviceId, ModbusAddress.HoldingRegisters(1, 1, (ushort)Unsafe.SizeOf<int>()));


        // 读取
        while (true)
        {
            var gatewayReadResult = await gateway.TagReader.ReadAsync(tag, TestContext.CancellationToken);
            if (gatewayReadResult.IsFailed) Assert.IsTrue(gatewayReadResult.IsSuccess, gatewayReadResult.ToString());
            else Console.WriteLine(gatewayReadResult.Value);

            await Task.Delay(100, TestContext.CancellationToken);
        }


        // 写入
        for (int i = 0; i < 1000; i++)
        {
            var gatewayWiteResult = await gateway.TagWriter.WriteAsync(tag, i, TestContext.CancellationToken);
            if (gatewayWiteResult.IsFailed) Assert.IsTrue(gatewayWiteResult.IsSuccess, gatewayWiteResult.ToString());

            await Task.Delay(1000, TestContext.CancellationToken);
        }

        // 停止服务
        await gateway.StopAsync(TestContext.CancellationToken);
    }

    public TestContext TestContext { get; set; }
}
