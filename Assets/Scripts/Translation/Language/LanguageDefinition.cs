
using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageDefinition
{
    public Dictionary<string, LangDefParam> Data = new Dictionary<string, LangDefParam>();

    public bool ContainsKey(string key)
    {
        if (Data == null)
            return false;

        return Data.ContainsKey(key);
    }

    public void Rename(string oldKey, string newKey)
    {
        if (!ContainsKey(oldKey))
        {
            Debug.LogError("No item for key '" + oldKey + "' was found to rename!");
            return;
        }
        if (ContainsKey(newKey))
        {
            Debug.LogError("There is already an item for key '" + newKey + "' cannot rename!");
            return;
        }

        // Rename process.
        LangDefParam p = Data[oldKey];
        Data.Remove(oldKey);
        p.Key = newKey;
        Data.Add(newKey, p);
    }
}

[Serializable]
public class LangDefParam
{
    public string Key;
    public string[] Params;
    [TextArea(2, 10)]
    public string Desription;
}