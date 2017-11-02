using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectPresets : MonoBehaviour
{
    public static HitEffectPresets _Instance;

    public GameObject Prefab;
    public SpriteListGodWhy[] Presets;

    public void Awake()
    {
        _Instance = this;
    }

    public Sprite[] GetSprites(int index)
    {
        return Presets[index].Sprites;
    }
}

[System.Serializable]
public class SpriteListGodWhy
{
    public Sprite[] Sprites;
}