
using UnityEngine;

public static class LanguageIO
{
    public static string LanguageFolder
    {
        get
        {
            return Application.streamingAssetsPath + "/Languages/Instances/";
        }
    }

    public static string LanguageDefinitionFolder
    {
        get
        {
            return Application.streamingAssetsPath + "/Languages/Definitions/";
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

        string path = GetLanguagePath(lang.Name);

        OutputUtils.ObjectToFile(lang, path);
    }

    public static Language LoadLanguage(string name)
    {
        return InputUtils.FileToObject<Language>(GetLanguagePath(name));
    }

    public static void SaveDefinition(LanguageDefinition def)
    {
        if (def == null)
            return;

        string path = GetDefinitionPath(def.Name);

        OutputUtils.ObjectToFile(def, path);
    }

    public static LanguageDefinition LoadDefinition(string name)
    {
        return InputUtils.FileToObject<LanguageDefinition>(GetDefinitionPath(name));
    }
}