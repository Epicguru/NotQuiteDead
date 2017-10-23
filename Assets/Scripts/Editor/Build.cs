using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;

public static class Build
{
    public const string GameName = "NQD";
    public const string BasePath = "Builds/";

    public static string[] GetLevels()
    {
        return new string[]
        {
            "Assets/Scenes/Setup V2.unity"
        };
    }

    [MenuItem( "Build/All" )]
    public static void BuildAll()
    {
        BuildWin32();
        BuildWin64();
    }

    [MenuItem("Build/Windows 32 bit")]
    public static void BuildWin32()
    {
        BuildPlayerOptions win32 = new BuildPlayerOptions();
        win32.locationPathName = BasePath + "Win32/" + GameName + ".exe";
        win32.target = BuildTarget.StandaloneWindows;
        win32.scenes = GetLevels();
        win32.options = BuildOptions.AllowDebugging | BuildOptions.Development;

        BuildPipeline.BuildPlayer(win32);
    }

    [MenuItem("Build/Windows 64 bit")]
    public static void BuildWin64()
    {
        BuildPlayerOptions win64 = new BuildPlayerOptions();
        win64.locationPathName = BasePath + "Win64/" + GameName + ".exe";
        win64.target = BuildTarget.StandaloneWindows64;
        win64.scenes = GetLevels();
        win64.options = BuildOptions.AllowDebugging | BuildOptions.Development;

        BuildPipeline.BuildPlayer(win64);
    }
}
