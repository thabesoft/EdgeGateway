using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.App.Services;

public interface IAliasService<TId>
    where TId : IEquatable<TId>
{
    string? Get(TId id);
    void Set(TId id, string alias);
}

public sealed class AliasService<TId> : IAliasService<TId> where TId : IEquatable<TId>
{
    private readonly ConcurrentDictionary<TId, string> _aliases = [];

    public AliasService() { }
    public static AliasService<TId> Instance { get; } = new();


    public string? Get(TId id)
    {
        _aliases.TryGetValue(id, out var value);
        return value;
    }
    public void Set(TId id, string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            _aliases.TryRemove(id, out _);
            return;
        }

        _aliases[id] = alias;
    }
}