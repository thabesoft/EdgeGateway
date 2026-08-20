using EdgeGateway.Data;
using EdgeGateway.Devices;
using EdgeGateway.Ports;

namespace EdgeGateway.Tags;


public record class Tag<T>
{
    public required PortAddress PortAddress { get; init; }
    public required DeviceId DeviceId { get; init; }
    public required DataDescriptor<T> DataDescriptor { get; init; }


    /// <summary>
    /// 返回标签的字符串表示。
    /// </summary>
    /// <returns>
    /// 格式为 <c>端口地址/设备名称 [数据地址] (数据类型)</c>，例如：
    /// <list type="bullet">
    /// <item><description><c>SerialPort[COM3]/温度传感器 [HR1] (Int32)</c></description></item>
    /// <item><description><c>IP[192.168.1.100:502]/压力表 [HR3[2]] (Float)</c></description></item>
    /// </list>
    /// </returns>
    public override string ToString()
    {
        return $"{PortAddress}/{DeviceId} [{DataDescriptor.Address}] {typeof(T).Name}";
    }
}

public static class Tag
{
    public static Tag<T> Create<T>(PortAddress portAddress, DeviceId deviceId, DataDescriptor<T> dataDescriptor)
    {
        return new Tag<T>()
        {
            PortAddress = portAddress,
            DeviceId = deviceId,
            DataDescriptor = dataDescriptor
        };
    }

    public static Tag<T> Create<T>(PortAddress portAddress, DeviceId deviceId, IDataAddress dataAddress, ITypeDescriptor<T> typeDescriptor)
    {
        return new Tag<T>()
        {
            PortAddress = portAddress,
            DeviceId = deviceId,
            DataDescriptor = DataDescriptor.Create(dataAddress, typeDescriptor)
        };
    }
}