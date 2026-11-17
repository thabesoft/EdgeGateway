namespace ThabeSoft.EdgeGateway.Recoveries;


internal sealed class PortRecoveryPolicyHandler(IEnumerable<IRecoveryPolicy> policies)
{
    private readonly IRecoveryPolicy[] _policies = policies?.Where(x => x is not null).ToArray() ?? [];


    public bool Handle(Exception exception)
    {
        foreach (var policy in _policies)
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