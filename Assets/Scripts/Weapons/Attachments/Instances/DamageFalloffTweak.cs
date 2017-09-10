using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DamageFalloffTweak : AttachmentTweak
{
    [Tooltip("Value between -1 and 1. Negative values INCREASE damage falloff. Positive values DECREASE damage falloff.")]
    public float Change = 0f;

    public override void Apply(Attachment x)
    {
        x.Effect_DamageFalloff(Change);
    }

    public override void Remove(Attachment x)
    {
        x.Reset_DamageFalloff();
    }

    public override string GetEffects()
    {
        string s = "";
        bool positive = Change >= 0f; // Less than zero means MORE damage falloff, which is worse!
        s += "Damage Falloff " + RichText.InColour((positive ? "-" : "+") + (int)(Mathf.Abs(Change) * 100f) + "%", positive ? Color.green : Color.red);
        return s;
    }
}
