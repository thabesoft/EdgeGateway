namespace ThabeSoft.EdgeGateway.Recoveries;

public interface IRecoveryPolicyFactory
{
    bool CanCreate(IRecoveryPolicyDescriptor descriptor);
    IRecoveryPolicy Create(IRecoveryPolicyDescriptor descriptor);
}