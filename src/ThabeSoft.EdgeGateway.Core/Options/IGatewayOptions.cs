namespace ThabeSoft.EdgeGateway.Options;


public sealed class GatewayOptions
{
    internal bool IsDisabledAutoLauch { get; private set; }
    internal bool IsDisabledConfigurator { get; private set; }


    public GatewayOptions DisabledAutoLauch()
    {
        IsDisabledAutoLauch = true;
        return this;
    }

    public GatewayOptions DisabledConfigurator()
    {
        IsDisabledConfigurator = true;
        return this;
    }
}