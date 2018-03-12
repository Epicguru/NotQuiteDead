
using UnityEngine;

public static class LanguageIO
{
    public static string LanguageFolder
    {
        get
        {
            return Application.streamingAssetsPath;
        }
    }

    public static void Test()
    {
        Debug.Log(LanguageFolder);
    }
}