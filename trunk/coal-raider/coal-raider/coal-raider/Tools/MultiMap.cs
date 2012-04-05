using System;
using System.Collections.Generic;

public class MultiMap<T,V>
{
    // 1
    Dictionary<T, List<V>> _dictionary = new Dictionary<T, List<V>>();

    // 2
    public void Add(T key, V value)
    {
        List<V> list;
        if (this._dictionary.TryGetValue(key, out list))
        {
            // 2A.
            list.Add(value);
        }
        else
        {
            // 2B.
            list = new List<V>();
            list.Add(value);
            this._dictionary[key] = list;
        }
    }

    // 3
    public IEnumerable<T> Keys
    {
        get
        {
            return this._dictionary.Keys;
        }
    }

    public IEnumerable<V> Values
    {
        get
        {
            List<V> _list = new List<V>();
            foreach (List<V> l in this._dictionary.Values)
            {
                _list.AddRange(l);
            }
            return _list;
        }
    }

    // 4
    public List<V> this[T key]
    {
        get
        {
            List<V> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                return list;
            }
            else
            {
                return new List<V>();
            }
        }
    }
}