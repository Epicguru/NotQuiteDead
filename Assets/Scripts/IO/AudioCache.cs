using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class AudioCache
{
    private static Dictionary<string, AudioClip> audio = new Dictionary<string, AudioClip>();
    private const string ItemPath = "Audio/Items/";

    public static void Release()
    {
        // Releases all cached audio clips.
        foreach(AudioClip c in audio.Values)
        {
            Resources.UnloadAsset(c);
        }

        audio.Clear();
    }

    public static AudioClip GetItemClip(string path, bool cache = true)
    {
        bool cached = audio.ContainsKey(path);

        if(!cached)
        {
            // Need to load into memory and return.
            AudioClip loaded = Resources.Load<AudioClip>(ItemPath + path);
            if(loaded == null)
            {
                Debug.LogError("Unable to load audio clip '" + ItemPath + path + "'");
                return null;
            }

            // Cache if wanted.
            if (cache)
            {
                audio.Add(path, loaded);
                //Debug.Log("Chached clip '" + path + "', #" + audio.Count);
            }

            return loaded;
        }
        else
        {
            return audio[path];
        }
    }
}
