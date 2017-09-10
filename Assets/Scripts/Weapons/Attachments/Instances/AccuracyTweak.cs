using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuracyTweak : AttachmentTweak
{
    public float Multiplier = 1f;

    public override void Apply(Attachment x)
    {
        x.Effect_Accuracy(Multiplier);
    }

    public override void Remove(Attachment x)
    {
        x.Reset_Accuracy();
    }

    public override string GetEffects()
    {
        string s = "";
        bool positive = Multiplier <= 1f; // Smaller means less spread, which is better!
        s += "Accuracy " + RichText.InColour((positive ? "+" : "-") + (int)(Mathf.Abs(Multiplier - 1f) * 100.01f) + "%", positive ? Color.green : Color.red);
        return s;
    }
}