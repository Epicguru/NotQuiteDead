
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public int MAX_SAUCES = 200;
    public Transform Parent;
    public GameObject Prefab;

    public List<AudioSauce> Sauces = new List<AudioSauce>();

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public AudioSauce PlayOneShot(Vector2 position, AudioClip clip, float volume, float pitch)
    {
        return PlayOneShot(position, clip, volume, pitch, 20, 10);
    }

    public AudioSauce PlayOneShot(Vector2 position, AudioClip clip, float volume, float pitch, float range, float panMagnitude)
    {
        AudioSauce s = GetNextSauce();
        if(s == null)
        {
            Debug.LogError("All audio sauces have been used up! (" + Sauces.Count + ", max " + MAX_SAUCES + ")");
            return s;
        }

        s.Clip = clip;
        s.Volume = volume;
        s.Pitch = pitch;
        s.Range = range;
        s.FullPanDistance = panMagnitude;
        s.transform.position = position;

        s.Play();

        return s;
    }

    public AudioSauce NewSauce()
    {
        AudioSauce s = Instantiate(Prefab, Parent).GetComponent<AudioSauce>();
        return s;
    }

    public AudioSauce GetNextSauce()
    {
        // Gets the next available audio sauce.
        // Or creates a new one if possible.

        foreach(AudioSauce s in Sauces)
        {
            if (s == null)
                continue;
            if (!s.IsPlaying)
                return s;            
        }

        if(Sauces.Count < MAX_SAUCES)
        {
            AudioSauce s = NewSauce();
            Sauces.Add(s);
            return s;
        }

        return null;
    }
}