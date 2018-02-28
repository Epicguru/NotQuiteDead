
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
public class ItemDataX
{
    [ReadOnly]
    public bool Created = false;

    [JsonProperty]
    private Dictionary<string, object> values = new Dictionary<string, object>();

    public void Add(string key, object value)
    {
        if (values.ContainsKey(key))
        {
            Debug.LogError("This item data already contains the key '" + key + "'!");
            return;
        }
        else
        {
            values.Add(key, value);
        }
    }

    public void Update(string key, object value, bool createIfMissing = true)
    {
        if (!values.ContainsKey(key))
        {
            if (createIfMissing)
            {
                Add(key, value);
                return;
            }
            else
            {
                Debug.LogError("Cannot update value for key '" + key + "', the key is not present!");
                return;
            }
        }
        else
        {
            values[key] = value;
        }
    }

    public bool ContainsKey(string key)
    {
        return values.ContainsKey(key);
    }

    public T Get<T>(string key)
    {
        if (ContainsKey(key))
        {
            try
            {
                T x = (T)values[key];
                return x;
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Invalid cast in item data for key '" + key + "': Value is of type '" + values[key].GetType().ToString() + "', requested type was '" + typeof(T).ToString() + "'!\n" + e);
                return default(T);
            }
        }
        else
        {
            return default(T);
        }
    }

    public T Get<T>(string key, T defaultValue)
    {
        if (ContainsKey(key))
        {
            try
            {
                T x = (T)values[key];
                return x;
            }
            catch (InvalidCastException e)
            {
                Debug.LogError("Invalid cast in item data for key '" + key + "': Value is of type '" + values[key].GetType().ToString() + "', requested type was '" + typeof(T).ToString() + "'!\n" + e);
                return defaultValue;
            }
        }
        else
        {
            return defaultValue;
        }
    }
}