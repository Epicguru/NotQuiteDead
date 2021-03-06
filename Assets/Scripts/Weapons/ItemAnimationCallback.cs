﻿using System;
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
            AudioManager.Instance.PlayOneShot(transform.position, c, 0.5f, 1f);
        }
    }
}
