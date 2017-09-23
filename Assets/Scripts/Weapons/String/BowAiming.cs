using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BowAiming : RotatingWeapon
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

    public override bool ShouldRotateNow()
    {
        return true; // TODO FIXME
    }

    public void Start()
    {
        Bow = GetComponent<Bow>();
    }    
}
