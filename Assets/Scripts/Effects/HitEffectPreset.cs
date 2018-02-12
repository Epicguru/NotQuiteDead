
using UnityEngine;

[CreateAssetMenu(fileName = "Hit Effect", menuName = "Effects/Hit Effect", order = 1)]
public class HitEffectPreset : ScriptableObject
{
    public Sprite[] Sprites;
    public HitEffectLightMode LightMode = HitEffectLightMode.UNLIT;
    public Color Colour = Color.white;
    public float FPS = 20f;
}

public enum HitEffectLightMode
{
    UNLIT,
    LIT,
    LIGHT
}