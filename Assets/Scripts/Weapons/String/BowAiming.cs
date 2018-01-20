using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BowAiming : RotatingItem
{
    public Bow Bow;

    public override float GetAimTime()
    {
        return Bow.DrawTime;
    }

    public override float GetCurvedTime(float rawTime)
    {
        return Bow.DrawCurve.Evaluate(rawTime);
    }

    public override bool ForceRotateNow()
    {
        return Bow.Released || Bow.InFire;
    }

    public override bool AllowRotateNow()
    {
        return true; // TODO FIXME
    }

    public void Start()
    {
        Bow = GetComponent<Bow>();
    }    
}
