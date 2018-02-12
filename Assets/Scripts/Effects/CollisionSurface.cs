
using UnityEngine;

public class CollisionSurface : MonoBehaviour
{
    [Header("Hit Effect")]
    public HitEffectPreset[] HitEffects;

    [Header("Audio")]
    public AudioClip[] HitSounds;
    public float Range = 20;
    [Range(0f, 1f)]
    public float PanRangePercentage = 0.2f;

    public virtual AudioClip GetAudioClip()
    {
        if (HitSounds == null || HitSounds.Length == 0)
            return null;
        return HitSounds[Random.Range(0, HitSounds.Length)];
    }

    public virtual HitEffectPreset GetHitEffect()
    {
        if (HitEffects == null || HitEffects.Length == 0)
            return null;
        return HitEffects[Random.Range(0, HitEffects.Length)];
    }

    public virtual float GetRange()
    {
        return Range;
    }

    public virtual float GetMaxPanRange()
    {
        return Range * PanRangePercentage;
    }

    public virtual float GetMinPanRange()
    {
        return 0f;
    }

    public virtual float GetLowPassDistance()
    {
        return 0.75f;
    }

    public virtual float GetVolume()
    {
        return 1f;
    }

    public virtual float GetPitch()
    {
        return 1f;
    }
}