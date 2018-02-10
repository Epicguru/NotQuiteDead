
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(AudioLowPassFilter))]
[ExecuteInEditMode]
public class AudioSauce : MonoBehaviour
{
    // Yes, the name is a joke.
    // Also so it doesn't conflict with AudioSource.

    [Header("Basics")]
    public AudioClip Clip;
    public float Pitch = 1f;
    [Range(0f, 1f)]
    public float Volume = 1f;
    public bool PlayOnStart = false;

    [Header("Falloff")]
    public float Range = 100f;
    public AnimationCurve SoundFalloff = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("Pan")]
    public float PanMagnitude = 30f;
    [Range(0f, 1f)]
    public float PanRange = 0.85f;

    [Header("Low Pass")]
    [Range(10f, 22000f)]
    public float LowPassCutoff = 600;
    [Range(0f, 1f)]
    public float LowPassStart = 0.6f;
    public AnimationCurve LowPassCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Info")]
    [ReadOnly]
    public Transform Listener;
    [ReadOnly]
    public bool IsPlaying;

    public AudioSource Source { get; private set; }
    public AudioLowPassFilter LPF { get; private set; }

    public void Awake()
    {
        Source = GetComponent<AudioSource>();
        LPF = GetComponent<AudioLowPassFilter>();

        if(Source == null)
        {
            Debug.LogError("Audio Sauce's AudioSource component is null!");
            return;
        }

        if(LPF == null)
        {
            Debug.LogError("Audio Sauce's Low Pass Filter component is null!");
            return;
        }

        ConfigureSource(Source);
    }

    public void Start()
    {
        if(PlayOnStart && Clip != null)
        {
            Play();
        }
    }

    public void Play()
    {
        Play(Camera.main.transform);
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
        Source.Play();
        IsPlaying = true;
        Update();
    }

    public void Update()
    {
        if (Range < 0)
            Range = 0f;

        if (IsPlaying)
        {
            if(Listener == null)
            {
                IsPlaying = false;
                Source.Stop();
                // TODO destroy obj.
            }

            Source.volume = GetVolume(Listener.position);
            Source.pitch = GetPitch(Listener.position);
            Source.panStereo = GetPan(Listener.position);
            LPF.cutoffFrequency = GetLowPassFrequency(Listener.position);

            if (!Source.isPlaying)
                IsPlaying = false;
        }
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

        x *= this.Volume;

        return x;
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

        x = Mathf.Clamp(x, -PanRange, PanRange);

        return x;
    }

    public virtual float GetPitch(Vector2 listener)
    {
        // For now leave as-is.
        return this.Pitch * Time.timeScale;
    }

    public virtual float GetLowPassFrequency(Vector2 listener)
    {
        const float MAX_FREQ = 22000f;
        float MIN_FREQ = LowPassCutoff;

        float distance = Vector2.Distance(transform.position, listener);
        float effectStartDst = Range * LowPassStart;

        // Out of range?
        if(distance > Range)
        {
            return MIN_FREQ;
        }

        // Before effect start distance?
        if(distance < effectStartDst)
        {
            return MAX_FREQ;
        }

        float dst = distance - effectStartDst;
        float maxDst = Range - effectStartDst;

        float p = Mathf.Clamp(dst / maxDst, 0f, 1f);
        float x = Mathf.Clamp(LowPassCurve.Evaluate(p), 0f, 1f);
        // Value of 0 means lowest freq.
        // Value of 1 means max freq.

        float frequency = Mathf.Lerp(MIN_FREQ, MAX_FREQ, x);

        return frequency;
    }

    public virtual void ConfigureSource(AudioSource source)
    {
        source.Stop();
        source.spatialBlend = 0f;
        source.loop = false;
        source.playOnAwake = false;
        source.bypassEffects = false;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Range);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range * LowPassStart);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, PanMagnitude);
    }
}