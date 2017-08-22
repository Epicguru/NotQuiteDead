using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class OutputUtils
{
    private const char S = '/';
    private const bool LOG_ERROR = false;

    // FILE PATHS

    public static string SaveDirectory
    {
        get
        {
            return Application.persistentDataPath; // Change me whenever!
        }
    }

    public static string InputSaveDirectory
    {
        get
        {
            return SaveDirectory + S + "Input";
        }
    }

    public static string InputSaveKeyBindings
    {
        get
        {
            return InputSaveDirectory + S + "Key Bindings.txt";
        }
    }

    // METHODS

    public static string ObjectToJson(object obj, bool prettyPrint = true)
    {
        if (obj == null)
            return string.Empty;
        return JsonUtility.ToJson(obj, prettyPrint);
    }

    public static void JsonToFile(string json, string path)
    {
        EnsureFileExists(path);
        File.WriteAllText(path, json);
    }

    public static void ObjectToFile(object obj, string path)
    {
        string json = ObjectToJson(obj);
        JsonToFile(json, path);
    }

    public static bool EnsureDirectoryExists(string dir)
    {
        if (Directory.Exists(dir))
            return false;

        Directory.CreateDirectory(dir);
        return true;
    }

    public static bool EnsureFileExists(string path)
    {
        if (File.Exists(path))
            return false;
        EnsureDirectoryExists(Path.GetDirectoryName(path));
        File.Create(path);
        return true;
    }
}

public class SerializableDictionary<K, V>
{
    public K[] Keys;
    public V[] Values;

    public SerializableDictionary() { }
    public SerializableDictionary(Dictionary<K, V> data)
    {
        this.Set(data);
    }

    public void Set(Dictionary<K, V> data)
    {
        Keys = new K[data.Count];
        Values = new V[data.Count];

        int i = 0;
        foreach(K key in data.Keys)
        {
            Keys[i] = key;
            Values[i] = data[key];
            i++;
        }
    }

    public Dictionary<K, V> ToDictionary()
    {
        if (Keys == null || Values == null)
            return null;

        Dictionary<K, V> d = new Dictionary<K, V>();
        int i = 0;
        foreach(K key in Keys)
        {
            d.Add(key, Values[i++]);
        }

        return d;
    }
}
