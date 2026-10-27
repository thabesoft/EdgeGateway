#if NETSTANDARD2_0

using System.Collections.ObjectModel;

namespace System;

public static class CollectionExtensions
{
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
        public IReadOnlyDictionary<TKey, TValue> AsReadOnly()
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }
    }
}

#endif