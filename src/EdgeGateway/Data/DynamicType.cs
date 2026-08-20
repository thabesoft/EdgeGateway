using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace EdgeGateway.Data;


/// <summary>
/// 动态类型，表示一个可以在运行时动态添加属性的对象
/// </summary>
public record class DynamicType : IReadOnlyDictionary<string, object>
{
    private readonly Dictionary<string, object> _properties = [with(StringComparer.OrdinalIgnoreCase)];


    public object this[string key] => _properties[key];
    public IEnumerable<string> Keys => _properties.Keys;
    public IEnumerable<object> Values => _properties.Values;
    public int Count => _properties.Count;


    public bool ContainsKey(string key) => _properties.ContainsKey(key);
    public bool TryGetValue(string key, [NotNullWhen(true)] out object value) => _properties.TryGetValue(key, out value!);


    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _properties.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}