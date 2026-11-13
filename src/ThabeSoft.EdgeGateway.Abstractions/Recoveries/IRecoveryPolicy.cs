using System.Net.Sockets;

namespace ThabeSoft.EdgeGateway.Recoveries;


public interface IRecoveryPolicy
{
    bool? CanRecover(Exception exception);
}