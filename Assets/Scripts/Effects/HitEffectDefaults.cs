
using UnityEngine;

public class HitEffectDefaults : MonoBehaviour
{
    public static HitEffectDefaults Instance;

    public HitEffectPreset Sparks;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }
}