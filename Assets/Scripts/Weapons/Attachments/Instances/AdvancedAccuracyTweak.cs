using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Attachment))]
public class AdvancedAccuracyTweak : AttachmentTweak
{
    public float X = 1f, Y = 1f;

    public override void Apply(Attachment a)
    {
        a.GetGun().Shooting.Damage.Inaccuracy.x *= X;
        a.GetGun().Shooting.Damage.Inaccuracy.y *= Y;
    }    

    public override void Remove(Attachment a)
    {
        a.Reset_Accuracy();
    }

    public override string GetEffects()
    {
        string s = "";
        bool bestPositive = X <= 1f;
        bool worstPositive = Y <= 1f; // Smaller is better!
        s += "Best Accuracy " + RichText.InColour((bestPositive ? "+" : "-") + (int)(Mathf.Abs(X - 1f) * 100.01f) + "%", bestPositive ? Color.green : Color.red) + ", ";
        s += "Worst Accuracy " + RichText.InColour((worstPositive ? "+" : "-") + (int)(Mathf.Abs(Y - 1f) * 100.01f) + "%", worstPositive ? Color.green : Color.red);
        return s;
    }
}