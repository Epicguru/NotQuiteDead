
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
public class ItemDataX
{
    public static JsonSerializerSettings Settings
    {
        get
        {
            if(_settings == null)
            {
                CreateSettings();
            }

            return _settings;
        }
        set
        {
            _settings = value;
        }
    }

    private static JsonSerializerSettings _settings;
    private static void CreateSettings()
    {
        _settings = new JsonSerializerSettings();

        _settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        _settings.ContractResolver = UnityContractResolver.Instance;
    }

    public ItemDataX()
    {

    }

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

    public T Get<T>(string key, T defaultValue, bool addIfMissing = false)
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
            if (addIfMissing)
            {
                Add(key, defaultValue);
            }

            return defaultValue;
        }
    }

    public void Remove(string key)
    {
        if (!ContainsKey(key))
        {
            Debug.LogError("Item data does not contain entry '" + key + "' cannot remove it!");
            return;
        }

        values.Remove(key);
    }

    public string Serialize(bool pretty = false)
    {
        Settings.Formatting = pretty ? Formatting.Indented : Formatting.None;

        return JsonConvert.SerializeObject(this, Settings);
    }

    public static ItemDataX Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogError("Null or whitespace-only input json! Cannot make item data from that!");
            return null;
        }

        try
        {
            ItemDataX itemData = JsonConvert.DeserializeObject<ItemDataX>(json, Settings);
            return itemData;
        }
        catch (Exception e)
        {
            Debug.LogError("Error deserializing json to create item data:\n" + json + "\nException Info:\n" + e);
            return null;
        }
    }
}