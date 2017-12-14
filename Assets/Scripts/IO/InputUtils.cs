using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class InputUtils
{
    private static readonly bool LOG_ERROR = false;

    public static string FileToText(string path)
    {
        if (!File.Exists(path))
        {
            if (LOG_ERROR) Debug.LogError("File not found when reading Json: " + path);
            return null;
        }

        return File.ReadAllText(path);
    }

    public static T JsonToObject<T>(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            if(LOG_ERROR) Debug.LogError("Json string input is empty or null!");
            return default(T);
        }

        return JsonConvert.DeserializeObject<T>(json);
    }

    public static T FileToObject<T>(string path)
    {
        string file = FileToText(path);

        if (file == null)
            return default(T);

        return JsonToObject<T>(file);
    }
}
