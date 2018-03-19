using UnityEngine;

public class ReloadSpeedTweak : AttachmentTweak
{
    public float Multiplier = 1f;

    public override void Apply(Attachment a)
    {
        a.Effect_ReloadSpeed(Multiplier);
    }

    public override string GetEffects()
    {
        bool positive = Multiplier > 1f;

        return "Reload Speed " + RichText.InColour(((positive ? "+" : "") + Mathf.RoundToInt((Multiplier - 1f) * 100f) + "%"), positive ? Color.green : Color.red);
    }

    public override void Remove(Attachment a)
    {
        a.Reset_ReloadSpeed();
    }
}