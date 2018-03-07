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
    private static string PersistentDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);


    // FILE PATHS

    public static string SaveDirectory
    {
        get
        {
            return PersistentDataPath + S + "My Games" + S + "Not Quite Dead" + S; // Change me whenever!
        }
    }

    public static string ItemSaveDirectory
    {
        get
        {
            return S + "Items" + S;
        }
    }

    public static string WorldItemSaveFile
    {
        get
        {
            return ItemSaveDirectory + "Dropped Items.txt";
        }
    }

    public static string InventoryItemSaveFile
    {
        get
        {
            return ItemSaveDirectory + "Inventory Items.txt";
        }
    }

    public static string GearItemSaveFile
    {
        get
        {
            return ItemSaveDirectory + "Gear Items.txt";
        }
    }

    public static string HeldItemSaveFile
    {
        get
        {
            return ItemSaveDirectory + "Hand Item.txt";
        }
    }

    public static string PlayerSaveDirectory
    {
        get
        {
            return S + "Players" + S;
        }
    }

    public static string PlayerStateFile
    {
        get
        {
            return S + "State.txt";
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
        jsonSettings.ContractResolver = UnityContractResolver.Instance;
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

    public static void ObjectToFile(object obj, string path, JsonSerializerSettings settings = null)
    {
        string json = ObjectToJson(obj, settings);
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