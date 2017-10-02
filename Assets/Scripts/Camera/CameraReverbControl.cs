using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraReverbControl : MonoBehaviour
{
    public AudioReverbPreset DefaultState;
    private AudioReverbFilter Filter;
    private float timer;

    public void Update()
    {
        if (Filter == null)
            Filter = GetComponent<AudioReverbFilter>();

        timer += Time.unscaledDeltaTime;

        if(timer > 0.1f)
            Filter.reverbPreset = DefaultState;
    }

    public void SetReverb(AudioReverbPreset preset)
    {
        timer = 0f;
        Filter.reverbPreset = preset;
    }
}
