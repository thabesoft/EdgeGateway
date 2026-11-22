using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.Configuration.Monitor;
using ThabeSoft.EdgeGateway.Configuration;

var probe = typeof(IConfigMonitor<GatewayConfig>);

var d = new ServiceCollection();
d.AddLogging();
d.AddPubSubOnly();

var asm = typeof(ThabeSoft.Configuration.DependencyInjection.DependencyInjectionExtensions).Assembly;
var binder = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
var m = typeof(ThabeSoft.Configuration.DependencyInjection.DependencyInjectionExtensions).GetMethods(binder)
    .Single(x => x.Name == "AddConfigFileMonitor").MakeGenericMethod(typeof(GatewayConfig));

Console.WriteLine("--- invoking AddConfigFileMonitor<GatewayConfig> via reflection ---");
var res = m.Invoke(null, [d, "Gateway.json", TimeSpan.FromSeconds(3)]);
Console.WriteLine($"returned collection count = {((IServiceCollection)res!).Count}");
Console.WriteLine($"exact match for IConfigMonitor<GatewayConfig> => {d.Count(x => x.ServiceType == probe)}");

foreach (var x in d)
    Console.WriteLine($"  {x.Lifetime,-10} {x.ServiceType.FullName}  impl={(x.ImplementationType?.FullName ?? "<factory>")}");

Console.WriteLine("\n--- manual TryAddSingleton(ServiceDescriptor) behavior ---");
var d2 = new ServiceCollection();
var desc = ServiceDescriptor.Singleton<IConfigMonitor<GatewayConfig>>(_ => throw new NotSupportedException());
Console.WriteLine($"desc.ServiceType = {desc.ServiceType}");
d2.TryAddSingleton(desc);
Console.WriteLine($"d2 count after TryAddSingleton = {d2.Count}, match={d2.Count(x => x.ServiceType == probe)}");

static class X
{
    public static IServiceCollection AddPubSubOnly(this IServiceCollection s)
        => ThabeSoft.PubSub.DependencyInjection.DependencyInjectionExtensions.AddPubSub(s);
}
