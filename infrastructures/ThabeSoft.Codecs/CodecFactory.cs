using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.Codecs;


internal sealed class CodecFactory(IServiceProvider services) : ICodecFactory
{
    // descriptor : codeec
    private readonly ConcurrentDictionary<object, object> _cache = new();

    public ICodec<T> Create<T>(CodecDescriptor<T> descriptor) where T : notnull
    {
        if (_cache.TryGetValue(descriptor, out var cachedCodec)) return (ICodec<T>)cachedCodec;

        var resolvers = services.GetServices<ICodecResolver<T>>();
        var resolver = resolvers.FirstOrDefault(x => x.CanResolve(descriptor))
            ?? throw new InvalidOperationException($"不支持创建的编码器({descriptor.GetType()}):{descriptor}");

        var codec = resolver.Resolve(descriptor);
        _cache.TryAdd(descriptor, codec);
        return codec;
    }
}