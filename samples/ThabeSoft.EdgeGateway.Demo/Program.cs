using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway;


await Host.CreateDefaultBuilder(args)
    .ConfigureServices(x => x
        .AddEdgeGateway()
        .AddSerial()
        .AddEthernet()
        .AddModbus()
        .AddDemo()
        )
    .RunConsoleAsync();