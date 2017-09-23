using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemAnimationCallback : MonoBehaviour
{
    public bool Active = true;
    private AudioSource source;

    [HideInInspector]
    public virtual AudioSource GetAudioSource()
    {
        AudioSource parent = GetComponentInParent<AudioSource>();
        return parent;
    }

    public void PlaySound(string sound)
    {
        AudioClip c = AudioCache.GetItemClip(sound);

        if (c != null)
        {
            if (source == null)
            {
                source = GetAudioSource();
            }
            source.PlayOneShot(c);
        }
    }
}
