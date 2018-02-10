
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSauce : MonoBehaviour
{
    [Header("Basics")]
    public AudioClip Clip;
    public float Pitch = 1f;
    [Range(0f, 1f)]
    public float Volume = 1f;

    [Header("Falloff")]
    [Range(1f, 1000f)]
    public float Range = 100f;
    public AnimationCurve SoundFalloff = AnimationCurve.Linear(0, 1, 1, 0);

    public AudioSource Source { get; private set; }

    public void Awake()
    {
        Source = GetComponent<AudioSource>();

        if(Source == null)
        {
            Debug.LogError("Audio Sauce's AudioSource component is null!");
            return;
        }

        ConfigureSource(Source);
    }

    public virtual void ConfigureSource(AudioSource source)
    {
        source.spatialBlend = 0f;
    }
}