using System.Collections;
using System.Collections.Generic;

namespace Talent.GraphEditor.Core
{
    public class BidirectionalDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, TValue> _forward = new();
        private Dictionary<TValue, TKey> _reverse = new();

        public IReadOnlyCollection<TKey> Keys { get { return _forward.Keys; } }
        public IReadOnlyCollection<TValue> Values { get { return _forward.Values; } }

        public void Add(TKey key, TValue value)
        {
            _forward.Add(key, value);
            _reverse.Add(value, key);
        }

        public void Set(TKey key, TValue value)
        {
            _forward[key] = value;
            _reverse[value] = key;
        }

        public TValue Get(TKey key)
        {
            return _forward[key];
        }

        public TKey Get(TValue key)
        {
            return _reverse[key];
        }

        public void Remove(TKey key)
        {
            _reverse.Remove(_forward[key]);
            _forward.Remove(key);
        }

        public void Remove(TValue key)
        {
            _forward.Remove(_reverse[key]);
            _reverse.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _forward.TryGetValue(key, out value);
        }

        public bool TryGetValue(TValue key, out TKey value)
        {
            return _reverse.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            return _forward.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _forward.ContainsValue(value);
        }

        public void Clear()
        {
            _forward.Clear();
            _reverse.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _forward.GetEnumerator();
        }
    }
}