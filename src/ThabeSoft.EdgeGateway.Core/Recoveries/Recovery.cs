using System.Net.Sockets;

namespace ThabeSoft.EdgeGateway.Recoveries;


public sealed class Recovery
{
    public static IRecoveryPolicyDescriptor IO(int maxCount)
    {
        return new OPExceptionRecoveryPolicyDescriptor() { MaxCount = maxCount };
    }
}



public sealed class OPExceptionRecoveryPolicyDescriptor : IRecoveryPolicyDescriptor
{
    public required int MaxCount { get; init; }

    public bool Equals(IRecoveryPolicyDescriptor other)
    {
        throw new NotImplementedException();
    }
}

internal sealed class IOExceptionRecoveryPolicyFactory : IRecoveryPolicyFactory
{
    public bool CanCreate(IRecoveryPolicyDescriptor descriptor)
    {
        return descriptor is OPExceptionRecoveryPolicyDescriptor;
    }

    public IRecoveryPolicy Create(IRecoveryPolicyDescriptor descriptor)
    {
        if (descriptor is not OPExceptionRecoveryPolicyDescriptor result)
            throw new InvalidOperationException("不支持的重试策略");

        return new IOExceptionRecoveryPolicy(result);
    }
}

internal sealed class IOExceptionRecoveryPolicy(OPExceptionRecoveryPolicyDescriptor descriptor) : IRecoveryPolicy
{
    private int _currentCount = 0;

    public bool? CanRecover(Exception exception)
    {
        if (exception is not IOException)
            return null;

        if (_currentCount + 1 > descriptor.MaxCount)
            return false;

        _currentCount++;
        return true;
    }
}