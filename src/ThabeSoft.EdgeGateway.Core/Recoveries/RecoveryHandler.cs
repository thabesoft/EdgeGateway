using System.Net.Sockets;

namespace ThabeSoft.EdgeGateway.Recoveries;


internal sealed class RecoveryHandler(IEnumerable<IRecoveryPolicy> policies)
{
    public async ValueTask<bool> HandleAsync(Exception exception, CancellationToken cancellationToken = default)
    {
        foreach (var policy in policies)
        {
            var canRecover = policy.CanRecover(exception);

            if (canRecover.HasValue)
            {
                return canRecover.Value;
            }
        }

        return false;
    }
}


public sealed class Recovery
{
    public static IRecoveryPolicyDescriptor SocketException(int maxCount)
    {
        return new SocketExceptionRecoveryPolicyDescriptor() { MaxCount = maxCount };
    }
}



public sealed class SocketExceptionRecoveryPolicyDescriptor : IRecoveryPolicyDescriptor
{
    public required int MaxCount { get; init; }

    public bool Equals(IRecoveryPolicyDescriptor other)
    {
        throw new NotImplementedException();
    }
}

internal sealed class SocketExceptionRecoveryPolicyFactory : IRecoveryPolicyFactory
{
    public bool CanCreate(IRecoveryPolicyDescriptor descriptor)
    {
        return descriptor is SocketExceptionRecoveryPolicyDescriptor;
    }

    public IRecoveryPolicy Create(IRecoveryPolicyDescriptor descriptor)
    {
        if (descriptor is not SocketExceptionRecoveryPolicyDescriptor result)
            throw new InvalidOperationException("不支持的重试策略");

        return new SocketExceptionRecoveryPolicy(result);
    }
}

internal sealed class SocketExceptionRecoveryPolicy(SocketExceptionRecoveryPolicyDescriptor descriptor) : IRecoveryPolicy
{
    private int _currentCount = 0;

    public bool? CanRecover(Exception exception)
    {
        if (exception is not SocketException)
            return null;

        if (_currentCount + 1 > descriptor.MaxCount)
            return false;

        _currentCount++;
        return true;
    }
}