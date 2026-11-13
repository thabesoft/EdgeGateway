using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Options;
using System.Net;
using ThabeSoft.EdgeGateway.Simulator.Options;

namespace ThabeSoft.EdgeGateway.Simulator.Configs;


internal sealed class ConfigScriptRunner(ISimulator simulator, IOptions<SimulatorOptions> options)
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var scriptFilePath = options.Value.ConfigFile;
        var scriptCode = await File.ReadAllTextAsync(scriptFilePath, cancellationToken);

        // 1. 配置脚本的引用和默认 using
        var scriptOptions = ScriptOptions.Default
            .AddReferences(
                typeof(ISimulator).Assembly,
                typeof(IPAddress).Assembly,
                typeof(Enumerable).Assembly
            )
            .AddImports(
                "System",
                "System.Net",
                "ThabeSoft.EdgeGateway"
            );

        // 2. 实例化全局上下文，把你的实例塞进去
        var globals = new ConfigScriptGlobals
        {
            Simulator = simulator
        };

        try
        {
            // 清空端口
            foreach (var i in simulator.Ports)
            {
                simulator.RemovePort(i.Endpoint);
            }

            // 3. 执行脚本
            await CSharpScript.RunAsync(
                code: scriptCode,
                options: scriptOptions,
                globals: globals,
                cancellationToken: cancellationToken
            );

            // 重新运行
            await simulator.StartAsync(cancellationToken);
        }
        catch (CompilationErrorException ex)
        {
            Console.WriteLine("脚本编译错误：");
            foreach (var diag in ex.Diagnostics)
                Console.WriteLine(diag);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"脚本执行异常：{ex.Message}");
        }
    }
}


// 脚本全局变量
public class ConfigScriptGlobals
{
    public ISimulator Simulator { get; set; } = default!;
}