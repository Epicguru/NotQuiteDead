using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineTweak : AttachmentTweak
{

    public float Multiplier = 1f;

    public override void Apply(Attachment x)
    {
        x.Effect_Magazine(Multiplier);
        Debug.Log("Applied magazine effect (" + name + ")");
    }

    public override void Remove(Attachment x)
    {
        x.Reset_Magazine();
    }

    public override string GetEffects()
    {
        string s = "";
        bool positive = Multiplier >= 1f;
        s += "Magazine Capacity " + RichText.InColour((positive ? "+" : "-") + (int)(Mathf.Abs(Multiplier - 1f) * 100.01f) + "%", positive ? Color.green : Color.red);
        return s;
    }
}
