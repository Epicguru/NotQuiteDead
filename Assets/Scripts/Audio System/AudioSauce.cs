
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

    [Header("Pan")]
    public float PanMagnitude = 30f;

    [Header("Info")]
    [ReadOnly]
    public Transform Listener;
    [ReadOnly]
    public bool Playing;

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
        if(Source == null)
        {
            Debug.LogError("Null AudioSource component, cannot play.");
            return;
        }
        if (Clip == null)
        {
            Debug.LogError("Null AudioClip, cannot play.");
            return;
        }

        this.Listener = listener; 
        Source.clip = this.Clip;

        Playing = true;
    }

    public void Update()
    {
        if (Range < 0)
            Range = 0f;
    }

    public virtual float GetVolume(Vector2 listener)
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

    public virtual float GetPan(Vector2 listener)
    {
        float offX = listener.x - transform.position.x;

        // When offX >= PanMagnitude, the sound plays exclusively in the left ear.
        // When offX <= -PanMagnitude, the sound plays exclusively in the right ear.

        // When offX / PanMag == 1, x = -1;
        // When offX / PanMag == -1, x = 1;

        float x = Mathf.Clamp(offX / PanMagnitude, -1f, 1f);
        x *= -1;

        // This is now the value that is played in each ear.
        // However, is sounds really weird if a sound plays exclusively in one ear. To fix that the value is restricted to a maximum range.
        // By default about 90% in one ear, 10% in the other.

        return 0f;
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