using ThabeSoft.EdgeGateway.Simulation.Networks;
using ThabeSoft.EdgeGateway.Simulation.ValueGenerators;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Simulation.Configuration;


public interface ISimulationConfigurator
{
    IModbusSimulationNetworkConfigurator AddNetwork(ModbusSimulatorNetworkDescriptor networkDescriptor);
}

public interface IModbusSimulationNetworkConfigurator
{
    IModbusSimulatorConfigurator AddSlave(byte unitId);
}

public interface IModbusSimulatorConfigurator
{
    IModbusSimulatorConfigurator AddValueGenerator<T>(TagDescriptor<T> tagDescriptor, ITriggerDescriptor triggerDescriptor, IValueGenerator<T> valueGenerator) where T : notnull;
}