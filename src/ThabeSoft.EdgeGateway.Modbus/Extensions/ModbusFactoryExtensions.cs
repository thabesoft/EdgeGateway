using EdgeGateway.Byte;
using EdgeGateway.Modbus.Byte;
using NModbus.IO;
using ThabeSoft.EdgeGateway.Modbus;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace NModbus;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

internal static class ModbusFactoryExtensions
{
    extension(IModbusFactory factory)
    {
        public IModbusMaster Create(ModbusFrameFormat modbusFrame, IStreamResource streamResource)
        {
            if (!Enum.IsDefined(modbusFrame))
                throw new ArgumentOutOfRangeException(nameof(modbusFrame));

            ArgumentNullException.ThrowIfNull(streamResource);


            return modbusFrame switch
            {
                ModbusFrameFormat.Rtu => factory.CreateRtuMaster(streamResource),
                ModbusFrameFormat.Ascii => factory.CreateAsciiMaster(streamResource),
                ModbusFrameFormat.Ip => factory.CreateIpMaster(streamResource),
                _ => throw new InvalidOperationException(),
            };
        }

        public IModbusMaster Create(ModbusFrameFormat modbusFrame, IByteStream byteStream)
        {
            ArgumentNullException.ThrowIfNull(byteStream);

            var streamResource = new StreamResourceAdapter(byteStream);
            return factory.Create(modbusFrame, streamResource);
        }
    }
}
