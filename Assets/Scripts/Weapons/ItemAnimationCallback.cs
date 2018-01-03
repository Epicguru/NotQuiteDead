using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemAnimationCallback : MonoBehaviour
{
    public bool Active = true;

    public void PlaySound(string sound)
    {
        AudioClip c = AudioCache.GetItemClip(sound);

        if (c != null)
        {
            AudioSource.PlayClipAtPoint(c, transform.position, 1f);
        }
    }
}
