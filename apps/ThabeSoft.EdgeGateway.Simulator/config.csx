int deviceCount = 10;
int deviceValueCount = 1000;
var triggerDescriptor = Trigger.Interval(10);


for (int port = 2333; port <= 2337; port++)
{
    // 创建端口
    var network = Simulator.AddPort(Simulation.ModbusIpPort(Ethernet.Tcp(IPAddress.Loopback, port)));

    // 创建设备
    for (byte id = 1; id <= deviceCount; id++)
    {
        var slave = await network.AddDeviceAsync(Simulation.ModbusDevice(id));

        // 创建数据
        for (int i = 0; i < deviceValueCount; i++)
        {
            // 地址按 4 递增（Double 占 4 个寄存器）
            var tag = Modbus.HoldingRegister(id, (ushort)(i * 4), Codec.Double);

            // 直接内联生成器逻辑（利用 switch 表达式）
            var generator = (i % 8) switch
            {
                0 => Simulation.SineWaveValue(1, 2 * Math.PI / 20),
                1 => Simulation.SquareWaveValue(-1, 1, 10, 0.3),
                2 => Simulation.TriangleWaveValue(-1, 1, 20),
                3 => Simulation.SawtoothWaveValue(0, 100, 20),
                4 => Simulation.RandomValue(0, 100, seed: 42 + i),
                5 => Simulation.StepValue(0, 1, 50),
                6 => Simulation.ConstantValue(3.14),
                7 => Simulation.NoisySineWaveValue(1, 0, 20, 0.2),
                _ => throw new InvalidOperationException("未知的生成器类型")
            };

            // 写入标签
            await slave.ValueAsync(tag, triggerDescriptor, generator);
        }
    }
}

Console.WriteLine("5000 个点位配置加载完毕！");