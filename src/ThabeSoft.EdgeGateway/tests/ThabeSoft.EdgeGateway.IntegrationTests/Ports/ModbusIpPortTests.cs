using Microsoft.Extensions.DependencyInjection;
using ModbusSlaveSimulator;
using System.Net;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Ports;


[TestClass]
public class ModbusIpPortTests : GatewayTestBase
{
    protected override void OnConfigureServices(ServiceCollection descriptors)
    {
        descriptors.AddModbus();
        descriptors.AddEthernet();
        descriptors.AddModbusIpSimulator(x => x.IPEndPoint = new(IPAddress.Loopback, 0));
    }


    [TestMethod]
    public async Task Gateway_Should_Read_Continuous_Data_From_TcpSimulator()
    {
        // ================= 1. 准备环境 =================
        var simulator = Services.GetRequiredService<IModbusIpSimulator>();
        await simulator.StartAsync(TestContext.CancellationToken);
        simulator.SetHoldingRegister(0, 100); // 初始值 100

        var port = Gateway.AddPort(IpChannelDescriptor.Tcp(simulator.IPEndPoint));
        await port.OpenAsync(TestContext.CancellationToken);

        var device = port.AddDevice(ModbusProtocolDescriptor.Ip);
        await device.EnableAsync(TestContext.CancellationToken);

        var trigger = new SignalTrigger();
        var tag = ModbusTag.HoldingRegister(simulator.UnitId, 0, WordCodec.BigEndian);
        var tagMonitor = device.Register("温度", tag, trigger);

        // ================= 2. 第一次手动触发 =================
        await trigger.OnTickAsync(TestContext.CancellationToken);
        Assert.AreEqual((ushort)100, tagMonitor.Value, "第一次读取失败");
        Assert.AreEqual(TagValueState.Good, tagMonitor.State);

        // ================= 3. 模拟现场数据变化 =================
        simulator.SetHoldingRegister(0, 250); // 改变模拟器寄存器的值

        // ================= 4. 第二次手动触发 =================
        await trigger.OnTickAsync(TestContext.CancellationToken);
        Assert.AreEqual((ushort)250, tagMonitor.Value, "数据发生变化后，未能读取到新值");

        // ================= 5. 模拟设备掉线 =================
        await simulator.StopAsync(TestContext.CancellationToken);

        // ================= 6. 第三次手动触发 =================
        await trigger.OnTickAsync(TestContext.CancellationToken);
        // 验证网关没有崩，且质量码变成了 Bad
        Assert.AreEqual((ushort)250, tagMonitor.Value, "异常时不应丢失上一次的正确值");
        Assert.AreEqual(TagValueState.Bad, tagMonitor.State, "通讯断开后，状态应变为 Bad");
    }
}
