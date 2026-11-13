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