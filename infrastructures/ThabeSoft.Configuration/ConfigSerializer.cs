using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.Configuration.Mapping;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.Configuration;


internal sealed class ConfigSerializer(
        IConfigNodeParser parser,
        IServiceProvider services
    ) : IConfigSerializer
{
    public async ValueTask<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        var mapper = services.GetRequiredService<IConfigMapper<T>>();

        var root = await parser.ParseAsync(stream, cancellationToken: cancellationToken);
        return mapper.Map(root.Required("网关配置不可为空"));
    }

    public ValueTask SerializeAsync<T>(T config, Stream stream, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}