using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTweak : AttachmentTweak
{
    public float Multiplier = 1f;

    public override void Apply(Attachment x)
    {
        x.Effect_Damage(Multiplier);
    }

    public override void Remove(Attachment x)
    {
        x.Reset_Damage();
    }

    public override string GetEffects()
    {
        string s = "";
        bool positive = Multiplier >= 1f;
        s += "Damage " + RichText.InColour((positive ? "+" : "-") + Mathf.RoundToInt((Multiplier - 1f) * 100.0f) + "%", positive ? Color.green : Color.red);
        return s;
    }
}