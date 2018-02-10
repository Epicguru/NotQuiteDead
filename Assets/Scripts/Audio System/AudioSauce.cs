
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
    public float Range = 100f;
    public AnimationCurve SoundFalloff = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("Info")]
    [ReadOnly]
    public Transform Listener;

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

    public void Play(Transform listener)
    {
        if(listener == null)
        {
            Debug.LogError("Tried to play AudioSauce with null listener!");
            return;
        }
    }

    public void Update()
    {
        if (Range < 0)
            Range = 0f;
    }

    public float GetVolume(Vector2 listener)
    {
        float distance = Vector2.Distance(transform.position, listener);
        if (distance > Range)
            return 0f;

        float p = Mathf.Clamp(distance / Range, 0f, 1f);
        float x = SoundFalloff.Evaluate(p);

        if (x < 0)
            x = 0;

        return p;
    }

    public virtual void ConfigureSource(AudioSource source)
    {
        source.spatialBlend = 0f;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}