using EdgeGateway.Options.Serialization.Converters;
using System.IO.Ports;
using System.Net;
using System.Text.Json.Serialization;

namespace EdgeGateway.Options;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(SerialPortOptionsDto), typeDiscriminator: "SerialPort")]
[JsonDerivedType(typeof(TcpPortOptionsDto), typeDiscriminator: "Tcp")]
[JsonDerivedType(typeof(UdpPortOptionsDto), typeDiscriminator: "Udp")]
public abstract record PortOptionsDto;


/// <summary>
/// 串口配置
/// </summary>
public sealed record class SerialPortOptionsDto : PortOptionsDto
{
    [JsonConverter(typeof(CleanStringConverter))]
    public required string PortName { get; init; }

    public int BaudRate { get; init; } = 9600;

    public int DataBits { get; init; } = 8;

    [JsonConverter(typeof(JsonStringEnumConverter<Parity>))]
    public Parity Parity { get; init; } = Parity.None;

    [JsonConverter(typeof(StopBitsNumberConverter))]
    public StopBits StopBits { get; init; } = StopBits.One;
}



/// <summary>
/// 以太网端口配置
/// </summary>
public abstract record class EthernetPortOptionsDto : PortOptionsDto
{
    [JsonConverter(typeof(IPEndPointConverter))]
    public required IPEndPoint Remote { get; init; }

    [JsonConverter(typeof(IPEndPointConverter))]
    public IPEndPoint? Local { get; init; }
}

/// <summary>
/// 以太网-TCP端口配置
/// </summary>
public sealed record class TcpPortOptionsDto : EthernetPortOptionsDto;

/// <summary>
/// 以太网-UDP端口配置
/// </summary>
public sealed record class UdpPortOptionsDto : EthernetPortOptionsDto;