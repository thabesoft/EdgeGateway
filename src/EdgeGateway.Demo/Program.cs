using EdgeGateway;
using Microsoft.Extensions.Hosting;


await Host.CreateDefaultBuilder(args)
    .ConfigureServices(x => x
        .AddEdgeGateway()
        .AddSerial()
        .AddEthernet()
        .AddModbus()
        .AddDemo()
        )
    .RunConsoleAsync();