using System.Collections;
using System.Text.Json.Nodes;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Codecs.Words;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ethernet;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Modbus;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Protocols.Modbus;
using ThabeSoft.EdgeGateway.Serial;
using ThabeSoft.EdgeGateway.Subscriptions;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.EdgeGateway.Transports.Ethernet;
using ThabeSoft.EdgeGateway.Transports.Serial;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.Triggers.Interval;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Json;


internal static class JsonGatewayConfigMapper
{
    public static GatewayConfig AsGatewayConfig(this JsonNode root)
    {
        var ports = root.GetByPath("Ports")
            ?.AsArray()
            .Where(x => x is not null)
            .Select(x => x!.AsPortConfig())
            ?? [];

        return new GatewayConfig() { Ports = [.. ports] };
    }

    private static PortConfig AsPortConfig(this JsonNode root)
    {
        var descriptor = root.Required("端口描述符不存在")
            .AsPortDescriptor();

        var devices = root.GetByPath("Devices")?.AsArray()
            .Where(x => x is not null)
            .Select(x => x!.AsDeviceConfig())
            ?? [];

        return new() { Descriptor = descriptor, Devices = [.. devices] };
    }
    private static PortDescriptor AsPortDescriptor(this JsonNode root)
    {
        var portId = root.GetValueStringByPath("Id").Required("缺少端口Id")
            .AsPortId();

        var transport = root.GetByPath("Transport").Required("缺少端口传输配置")
            .AsTransportDescriptor();

        return new() { Id = portId, Transport = transport };
    }
    private static TransportDescriptor AsTransportDescriptor(this JsonNode root)
    {
        var transportType = root.GetValueStringByPath("Type").Required("缺少传输类型")
            .ToEnum<TransportType>();

        if (transportType == TransportType.Ethernet)
        {
            var ipProtocolType = root.GetValueStringByPath("ProtocolType").Required("缺少Ip协议类型")
                .ToEnum<IpProtocolType>();

            var ip = root.GetValueStringByPath("Ip").Required("缺少Ip地址")
                .ToIpAddress();

            var port = root.GetValueStringByPath("Port").Required("缺少端口")
                .ToInt32()
                .Positive("端口不可为负数");

            var retry = root.GetByPath("Retry")?.AsRetryDescriptor()
                ?? Retry.Default;


            return new IpTransportDescriptor()
            {
                ProtocolType = ipProtocolType,
                IP = ip,
                Port = port,
                Retry = retry
            };
        }

        if (transportType == TransportType.Serial)
        {
            var portName = root.GetValueStringByPath("PortName").Required("缺少串口名称")
                .PortName();

            var baudRate = root.GetValueStringByPath("BaudRate").Required("缺少波特率")
                .ToInt32()
                .StandardBaudRate();

            var dataBits = root.GetValueStringByPath("DataBits").Required("缺少数据位")
                .ToInt32()
                .DataBits();

            var parity = root.GetValueStringByPath("Parity").Required("缺少校验值")
                .ToEnum<SerialParity>();

            var stopBits = root.GetValueStringByPath("StopBits").Required("缺少停止位")
                .ToEnum<SerialStopBits>();

            var retry = root.GetByPath("Retry")?.AsRetryDescriptor()
                ?? Retry.Default;

            return new SerialTransportDescriptor()
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity,
                StopBits = stopBits,
                Retry = retry
            };
        }


        throw new InvalidOperationException("不支持的传输描述符");
    }
    private static RetryDescriptor AsRetryDescriptor(this JsonNode root)
    {
        var startInterval = root.GetValueStringByPath("StartInterval").Required("缺少起始间隔")
            .ToTimeSpan();

        var maxInterval = root.GetValueStringByPath("MaxInterval").Required("缺少最大间隔")
            .ToTimeSpan();

        var backoffMultiplier = root.GetValueStringByPath("BackoffMultiplier").Required("缺少退避倍数")
            .ToDouble()
            .AtLeast(1);

        return new(startInterval, maxInterval)
        {
            BackoffMultiplier = backoffMultiplier
        };
    }


    private static DeviceConfig AsDeviceConfig(this JsonNode root)
    {
        var descriptor = root.Required("设备描述符不存在")
            .AsDeviceDescriptor();

        var subscriptions = root.GetByPath("Subscriptions")?.AsArray()
            .Where(x => x is not null)
            .Select(x => x!.AsSubscriptionConfig(descriptor.Protocol.Type))
            ?? [];

        return new() { Descriptor = descriptor, Subscriptions = [.. subscriptions] };
    }
    private static DeviceDescriptor AsDeviceDescriptor(this JsonNode root)
    {
        var portId = root.GetValueStringByPath("Id").Required("缺少设备Id")
            .AsDeviceId();

        var protocol = root.GetByPath("Protocol").Required("缺少协议配置")
            .AsProtocolDescriptor();

        return new() { Id = portId, Protocol = protocol };
    }
    private static ProtocolDescriptor AsProtocolDescriptor(this JsonNode root)
    {
        var protocolType = root.GetValueStringByPath("Type").Required("缺少传输类型")
           .ToEnum<ProtocolType>();

        if (protocolType == ProtocolType.Modbus)
        {
            var frameFormat = root.GetValueStringByPath("FrameFormat").Required("缺少帧格式")
                .ToEnum<ModbusFrameFormat>();

            var transportType = root.GetValueStringByPath("TransportType").Required("缺少传输类型")
                .ToEnum<TransportType>();

            return new ModbusProtocolDescriptor(frameFormat, transportType);
        }

        throw new InvalidOperationException("不支持的协议");
    }


    private static SubscriptionConfig AsSubscriptionConfig(this JsonNode root, ProtocolType protocolType)
    {
        var (descriptor, type) = root.Required("订阅描述符不存在")
            .AsSubscriptionDescriptor(protocolType);

        return new() { Descriptor = descriptor, ValueType = type, Trigger = null! };
    }
    private static (SubscriptionDescriptor, CodecKind) AsSubscriptionDescriptor(this JsonNode root, ProtocolType protocolType)
    {
        var id = root.GetValueStringByPath("Id").Required("缺少订阅Id")
            .AsSubscriptionId();

        var trigger = root.GetByPath("Trigger").Required("缺少订阅Id")
            .AsTriggerDescriptor();

        var tagNode = root.GetByPath("Tag").Required("缺少订阅标签");
        var (tag, valueType) = protocolType switch
        {
            ProtocolType.Modbus => tagNode.AsModbusTagDescriptor(),
            _ => throw new InvalidOperationException("不支持的标签")
        };

        SubscriptionDescriptor a = valueType switch
        {
            CodecKind.Bool => Create<bool>(tag),
            CodecKind.Int16 => Create<short>(tag),
            CodecKind.Bits => Create<BitArray>(tag),
            _ => throw new InvalidOperationException("不支持的值类型")
        };
        return (a, valueType);

        SubscriptionDescriptor<T> Create<T>(TagDescriptor tag) where T : notnull =>
            new() { Id = id, Trigger = trigger, Tag = (TagDescriptor<T>)tag };
    }
    private static TriggerDescriptor AsTriggerDescriptor(this JsonNode root)
    {
        var type = root.GetValueStringByPath("Type").Required("缺少触发器类型")
            .ToEnum<TriggereType>();

        if (type == TriggereType.Interval)
        {
            var interval = root.GetValueStringByPath("Interval").Required("缺少触发间隔")
                .ToTimeSpan();

            return new IntervalTriggerDescriptor() { Interval = interval };
        }

        throw new InvalidOperationException("未知的触发器类型");
    }
    private static (TagDescriptor, CodecKind) AsModbusTagDescriptor(this JsonNode root)
    {
        var type = root.GetValueStringByPath("Type").Required("缺少标签类型")
            .ToEnum<CodecKind>();

        var kind = root.GetValueStringByPath("Kind").Required("缺少Modbu数据区域")
            .ToEnum<ModbusDataKind>();

        var slaveId = root.GetValueStringByPath("SlaveId").Required("缺少Modbu数据区域")
           .ToByte();

        var address = root.GetValueStringByPath("Address").Required("缺少Modbu数据地址")
           .ToUInt16();


        if (kind == ModbusDataKind.Coil && type == CodecKind.Bool)
        {
            var bitOffset = root.GetValueStringByPath("BitOffset").Required("缺少比特偏移")
                .ToUInt16();
            return (ModbusBuilder.Coil(slaveId, address, bitOffset), type);
        }
        if (kind == ModbusDataKind.Coil && type == CodecKind.Bits)
        {
            var quantity = root.GetValueStringByPath("Quantity").Required("缺少比特偏移")
                .ToUInt16();
            return (ModbusBuilder.Coils(slaveId, address, quantity), type);
        }


        if (kind == ModbusDataKind.DiscreteInput && type == CodecKind.Bool)
        {
            var bitOffset = root.GetValueStringByPath("BitOffset").Required("缺少比特偏移")
                .ToUInt16();
            return (ModbusBuilder.DiscreteInput(slaveId, address, bitOffset), type);
        }
        if (kind == ModbusDataKind.DiscreteInput && type == CodecKind.Bits)
        {
            var quantity = root.GetValueStringByPath("Quantity").Required("缺少比特偏移")
                .ToUInt16();
            return (ModbusBuilder.DiscreteInputs(slaveId, address, quantity), type);
        }


        if (kind == ModbusDataKind.HoldingRegister && type == CodecKind.Int16)
        {
            var endianess = root.GetValueStringByPath("Endianess").Required("缺少比特偏移")
                .ToEnum<WordEndianess>();

            return endianess switch
            {
                WordEndianess.Big => (ModbusBuilder.HoldingRegister(slaveId, address, WordCodec.BigEndian.AsShort()), type),
                WordEndianess.Little => (ModbusBuilder.HoldingRegister(slaveId, address, WordCodec.LittleEndian.AsShort()), type),
                _ => throw new InvalidOperationException("缺少端序")
            };
        }


        throw new InvalidOperationException("未知的触发器类型");
    }


    private static PortId AsPortId(this string id) => PortId.From(id);
    private static DeviceId AsDeviceId(this string id) => DeviceId.From(id);
    private static SubscriptionId AsSubscriptionId(this string id) => SubscriptionId.From(id);
}


public enum WordEndianess
{
    Big,
    Little
}