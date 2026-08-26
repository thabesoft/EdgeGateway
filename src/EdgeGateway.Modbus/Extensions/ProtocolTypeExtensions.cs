#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace EdgeGateway.Protocols;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class ProtocolTypeExtensions
{
    extension(ProtocolType)
    {
        public static ProtocolType ModbusRtu => ProtocolType.Parse("ModbusRtu");
        public static ProtocolType ModbusIp => ProtocolType.Parse("ModbusIp");
        public static ProtocolType ModbusAscii => ProtocolType.Parse("ModbusAscii");
    }
}
