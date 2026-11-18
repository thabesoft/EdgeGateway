using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.App.Desktop.Services;
using ThabeSoft.EdgeGateway.App.Services;

namespace ThabeSoft.EdgeGateway.App.Desktop.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayAppDesktop()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ISerialPortNamesProvider, SerialPortNamesProvider>());

            return descriptors;
        }
    }
}