using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeTweak : AttachmentTweak
{
    public float Multiplier = 1f;

    public override void Apply(Attachment x)
    {
        x.Effect_Range(Multiplier);
    }

    public override void Remove(Attachment x)
    {
        x.Reset_Range();
    }

    public override string GetEffects()
    {
        string s = "";
        bool positive = Multiplier >= 1f;
        s += "Range " + RichText.InColour((positive ? "+" : "-") + (int)(Mathf.Abs(Multiplier - 1f) * 100.01f) + "%", positive ? Color.green : Color.red);
        return s;
    }
}
