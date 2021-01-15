using System;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public static class DictionaryExtensions
    {
        public static TValue AddOrGetExisting<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : new()
        {
            return dictionary.AddOrGetExisting(key, new TValue());
        }

        public static TValue AddOrGetExisting<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> generator)
        {
            if (generator is null)
                throw new ArgumentNullException(nameof(generator));

            if (dictionary.TryGetValue(key, out TValue value))
                return value;

            value = generator();
            dictionary.Add(key, value);

            return value;
        }

        public static TValue AddOrGetExisting<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue instance)
        {
            if (dictionary.TryGetValue(key, out TValue value))
                return value;

            value = instance;
            dictionary.Add(key, value);

            return value;
        }
        
        public static TValue GetOrReturnNull<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out TValue value))
                return value;
            return default;
        }
    }
}
