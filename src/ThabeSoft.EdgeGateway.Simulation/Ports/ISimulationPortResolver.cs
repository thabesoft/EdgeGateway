namespace ThabeSoft.EdgeGateway.Ports;


public interface ISimulationPortResolver
{
    bool CanCreate(SimulatorPortCreationContext context);
    ISimulationPort Resolve(SimulatorPortCreationContext context);
}