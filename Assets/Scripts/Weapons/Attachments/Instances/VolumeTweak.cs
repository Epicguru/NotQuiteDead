using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeTweak : AttachmentTweak
{
    public Vector2 Multiplier = Vector2.one;

    public override void Apply(Attachment x)
    {
        //x.Effect_ShotVolume(Multiplier);
    }

    public override string GetEffects()
    {
        string s = "";
        float p = (Multiplier.x + Multiplier.y) / 2f;
        bool positive = p <= 1f; // Less sound is better?
        s += "Shot Loudness " + RichText.InColour("~ " + (positive ? "-" : "+") + (int)(Mathf.Abs(p - 1f) * 100.01f) + "%", positive ? Color.green : Color.red);
        return s;
    }

    public override void Remove(Attachment x)
    {
        //x.Reset_ShotVolume();
    }
}