using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Transport;


/// <summary>
/// 一条<b>有地址、可启停、可双向收发字节</b>的传输连接。
/// </summary>
/// <remarks>
/// <para>
/// <b>职责</b>：封装一条底层字节传输通道（串口 / TCP / 管道等），向协议层
/// （<see cref="IProtocolSession"/>）提供"启动后可读写字节"的能力。
/// 它组合了 <see cref="IByteDuplex"/>（双向字节收发）和
/// <see cref="ITransportLifecycle"/>（启停），并通过 <see cref="Endpoint"/> 标识其物理端点。
/// </para>
/// <para>
/// <b>层级</b>：<see cref="IProtocolSession"/> → <b>ITransportConnection</b> → <see cref="IByteDuplex"/>。
/// 协议会话在连接之上实现帧/校验/寻址；连接本身<b>不理解协议</b>，只负责字节搬运与生命周期。
/// </para>
/// <para>
/// <b>生命周期</b>：使用前必须 <see cref="ITransportLifecycle.StartAsync"/>；
/// 停止后不可再读写。实例通常由工厂创建，<b>创建者负责释放</b>（<see cref="IAsyncDisposable.DisposeAsync"/>）。
/// </para>
/// <para>
/// <b>线程安全</b>：<see cref="IByteReader"/> / <see cref="IByteWriter"/> 的方法
/// <b>不保证并发安全</b>；并发调用需由调用方串行化（或由协议会话内部加锁）。
/// </para>
/// </remarks>
public interface ITransportConnection : IStatefulLifecycle, IByteDuplex, IAsyncDisposable
{
    ConnectionEndpoint Endpoint { get; }
}