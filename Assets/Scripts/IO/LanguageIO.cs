
using UnityEngine;

public static class LanguageIO
{
    public static string LanguageFolder
    {
        get
        {
            return Application.dataPath + "/Resources/Languages/Instances/";
        }
    }

    public static string LanguageDefinitionFolder
    {
        get
        {
            return Application.dataPath + "/Resources/Languages/Definitions/";
        }
    }

    public static string GetLanguagePath(string langName)
    {
        return LanguageFolder + langName + ".txt";
    }

    public static string GetDefinitionPath(string defName)
    {
        return LanguageDefinitionFolder + defName + ".txt";
    }

    public static void SaveLanguage(Language lang)
    {
        if (lang == null)
            return;
        if (!Application.isEditor)
            return;

        string path = GetLanguagePath(lang.Name);

        OutputUtils.ObjectToFile(lang, path);
    }

    public static Language LoadLanguage(string name)
    {
        if (Application.isEditor)
        {
            // Load from absolute file path...
            return InputUtils.FileToObject<Language>(GetLanguagePath(name));
        }
        else
        {
            // Load from internal resources...
            string path = "Languages/Instances/" + name;
            TextAsset a = Resources.Load<TextAsset>(path);
            string json = a.text;
            return InputUtils.JsonToObject<Language>(json);
        }
    }

    public static void SaveDefinition(LanguageDefinition def)
    {
        if (def == null)
            return;
        if (!Application.isEditor)
            return;

        string path = GetDefinitionPath(def.Name);

        OutputUtils.ObjectToFile(def, path);
    }

    public static LanguageDefinition LoadDefinition(string name)
    {
        if (Application.isEditor)
        {
            // Load from absolute file path...
            return InputUtils.FileToObject<LanguageDefinition>(GetDefinitionPath(name));
        }
        else
        {
            // Load from internal resources...
            string path = "Languages/Definitions/" + name;
            TextAsset a = Resources.Load<TextAsset>(path);
            string json = a.text;
            return InputUtils.JsonToObject<LanguageDefinition>(json);
        }
    }
}