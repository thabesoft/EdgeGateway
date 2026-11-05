using BenchmarkDotNet.Running;
using ThabeSoft.EdgeGateway;

#if DEBUG
var a = new GatewayBenchmarks();
a.Setup();
await a.Run();
#else
BenchmarkRunner.Run<GatewayBenchmarks>();
#endif