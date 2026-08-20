using FluentResults;

namespace EdgeGateway.Ports;


public abstract class Port(PortAddress portAddress) : IPort
{
    public PortAddress Address => portAddress;
    public bool IsOpen { get; private set; }


    async ValueTask<Result> IPort.OpenAsync(CancellationToken cancellationToken)
    {
        if (IsOpen) return Result.Fail("当前已开启");

        var result = await OpenAsync(cancellationToken);
        if (result.IsFailed) return result;

        IsOpen = true;
        return Result.Ok();
    }
    async ValueTask<Result> IPort.CloseAsync(CancellationToken cancellationToken)
    {
        if (!IsOpen) return Result.Fail("当前已关闭");

        var result = await CloseAsync(cancellationToken);
        if (result.IsFailed) return result;

        IsOpen = false;
        return Result.Ok();
    }


    protected abstract ValueTask<Result> OpenAsync(CancellationToken cancellationToken = default);
    protected abstract ValueTask<Result> CloseAsync(CancellationToken cancellationToken = default);
    public abstract ValueTask DisposeAsync();


    public abstract ValueTask<Result> ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
    public abstract ValueTask<Result> WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
}

public abstract class StreamPort(PortAddress portAddress) : Port(portAddress)
{
    protected abstract Stream Stream { get; }

    public override ValueTask<Result> ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return Result.Try(() => Stream.ReadExactlyAsync(buffer, cancellationToken));
    }
    public override ValueTask<Result> WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return Result.Try(() => Stream.WriteAsync(data, cancellationToken));
    }
}