using Newtonsoft.Json;
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
            return Application.persistentDataPath + S; // Change me whenever!
        }
    }

    public static string InputSaveDirectory
    {
        get
        {
            return SaveDirectory + "Input" + S;
        }
    }

    public static string InputSaveKeyBindings
    {
        get
        {
            return InputSaveDirectory + "Key Bindings.txt";
        }
    }

    public static string RealitySaveDirectory
    {
        get
        {
            return SaveDirectory + "Realities" + S;
        }
    }

    public static string RealityMapDirectory
    {
        get
        {
            return S + "Map" + S;
        }
    }

    public static string RealityLayersDirectory
    {
        get
        {
            return RealityMapDirectory + "Layers" + S;
        }
    }

    public static string RealityChunksDirectory
    {
        get
        {
            return S + "Chunks" + S;
        }
    }

    // METHODS

    private static JsonSerializerSettings jsonSettings;

    public static JsonSerializerSettings GetJsonSettings()
    {
        if (jsonSettings == null)
            CreateDefaultSettings();

        return jsonSettings;
    }

    public static void CreateDefaultSettings()
    {
        jsonSettings = new JsonSerializerSettings();
        jsonSettings.Formatting = Formatting.Indented;
    }

    public static string ObjectToJson(object obj, JsonSerializerSettings settings = null)
    {
        if (obj == null)
            return string.Empty;
        return JsonConvert.SerializeObject(obj, settings == null ? GetJsonSettings() : settings);
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
        FileStream f = File.Create(path);
        f.Close();
        return true;
    }
}