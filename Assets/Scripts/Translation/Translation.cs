
using UnityEngine;

public static class Translation
{
    public const string DefaultLanguageName = "English";

    public static Language DefaultLanguage
    {
        get
        {
            if (_Default == null)
            {
                _Default = LanguageIO.LoadLanguage(DefaultLanguageName);
            }
            return _Default;
        }
    }
    private static Language _Default;

    private static Language Current;

    public static int TranslationCounter;

    public static Language GetCurrentLanguage()
    {
        if(Current == null)
        {
            return DefaultLanguage;
        }
        else
        {
            return Current;
        }
    }

    public static string GetCurrentLanguageVerbose()
    {
        var lang = GetCurrentLanguage();

        if(lang == null)
        {
            return "LANGUAGE_ERROR (Missing current language, including default)";
        }
        else
        {
            return lang.Name + " (" + lang.NativeName + ")" + (lang.IsDefault ? " (Default)" : "");
        }
    }

    public static void SetLanguage(string name)
    {
        if(name == DefaultLanguageName)
        {
            Current = DefaultLanguage;
        }
        else
        {
            Current = LanguageIO.LoadLanguage(name);
        }
    }

    public static string Translate(string key, params object[] args)
    {
        Language lang = GetCurrentLanguage();
        if(lang == null)
        {
            Debug.LogError("Serious translation error, null current language (default lang is null too!)");
            return "LANG_ERROR";
        }
        return lang.TryTranslate(key, args);
    }
}

public static class StaticTrans
{
    public static string Translate(this string key, params object[] args)
    {
        return Translation.Translate(key, args);
    }
}