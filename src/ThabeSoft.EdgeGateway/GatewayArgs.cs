using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway;


internal readonly struct GatewayArgs
{
    // Gateway
    public required ILogger Logger { get; init => field = value.Required(); }
    public required ILifecycleStateMachine GatwayStateMachine { get; init => field = value.Required(); }

    // Port
    public required PortManager PortManager { get; init => field = value.Required(); }
}