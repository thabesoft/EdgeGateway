using System.Text.Json.Serialization;

namespace EdgeGateway.Dto;


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
    public required string PortName { get; init; }

    public int BaudRate { get; init; } = 9600;
    public int DataBits { get; init; } = 8;
    public string Parity { get; init; } = "None";
    public double StopBits { get; init; } = 1;
}



/// <summary>
/// 以太网端口配置
/// </summary>
public abstract record class EthernetPortOptionsDto : PortOptionsDto
{
    public required string Remote { get; init; }
    public string? Local { get; init; }
}

/// <summary>
/// 以太网-TCP端口配置
/// </summary>
public sealed record class TcpPortOptionsDto : EthernetPortOptionsDto;

/// <summary>
/// 以太网-UDP端口配置
/// </summary>
public sealed record class UdpPortOptionsDto : EthernetPortOptionsDto;