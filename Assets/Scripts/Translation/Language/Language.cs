﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
public class Language
{
    public const string IS_DEFAULT_VALUE = "<UD>";

    [Header("Basic")]
    public string Name;
    public string NativeName;

    [Header("Info")]
    public string[] Authors;

    [JsonIgnore]
    public bool IsLoaded
    {
        get
        {
            return Data != null && Data.Count != 0;
        }
    }

    [JsonIgnore]
    public bool IsDefault
    {
        get
        {
            return Name == Translation.DefaultLanguageName;
        }
    }

    [JsonProperty]
    [HideInInspector]
    public Dictionary<string, string> Data;

    public virtual string GetDefault(string key, object[] args)
    {
        if (IsDefault)
        {
            return "Unimplemented <" + (key == null ? "null" : key) + '>';
        }
        else
        {
            return Translation.DefaultLanguage.TryTranslate(key, args);
        }
    }

    public bool ContainsKey(string key)
    {
        if (!IsLoaded)
        {
            return false;
        }

        if (!Data.ContainsKey(key))
        {
            return false;
        }

        return true;
    }

    public bool KeyIsTranslated(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }
        if (!ContainsKey(key))
        {
            return false;
        }

        string raw = Data[key];

        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }
        else
        {
            if(raw.Trim() == IS_DEFAULT_VALUE)
            {
                return false;
            }
            else
            {
                return !IsDefaultLang(key);
            }
        }
    }

    public bool IsDefaultLang(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }
        if (!ContainsKey(key))
        {
            return false;
        }
        string raw = Data[key];
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }
        return raw.TrimStart().StartsWith(IS_DEFAULT_VALUE);
    }

    public virtual string TryTranslate(string key, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError("Null or blank key used in language translate request.");
            return GetDefault(null, args);
        }
        key = key.Trim();
        if (!IsLoaded)
        {
            Debug.LogError("Language '" + this + "' is not loaded, cannot translate '" + key + "'.");
            return GetDefault(key, args);
        }

        if (!ContainsKey(key))
        {
            Debug.LogWarning("No translation offered for '" + key + "' in '" + this + "'");
            return GetDefault(key, args);
        }

        try
        {
            string raw = Data[key];

            if(raw == null)
            {
                Debug.LogWarning("No translation offered for '" + key + "' in '" + this + "'");
                return GetDefault(key, args);
            }
            else
            {
                if(args == null || args.Length == 0)
                {
                    // No args, just return the basic translated version.
                    return raw;
                }
                else
                {
                    // Use the args to format...
                    string translated = string.Format(raw, args);

                    return translated;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Exception translating '" + key + "' to '" + this + "'.");
            Debug.LogWarning(e);
            return GetDefault(key, args);
        }
    }

    public string PrettyAuthors()
    {
        if(Authors == null || Authors.Length == 0)
        {
            return "<none>";
        }
        string s = "";
        for (int i = 0; i < Authors.Length; i++)
        {
            string sep = ", ";
            if(i == Authors.Length - 2)
            {
                sep = " and ";
            }
            if(i == Authors.Length - 1)
            {
                sep = "";
            }
            s += Authors[i] == null ? "<null>" : Authors[i].Trim() + sep;
        }
        return s;
    }

    public override string ToString()
    {
        return (Name == null ? "<null>" : Name) + " by " + PrettyAuthors();
    }
}