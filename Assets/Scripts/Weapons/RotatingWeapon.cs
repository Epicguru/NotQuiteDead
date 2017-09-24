using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

public abstract class RotatingWeapon : NetworkBehaviour
{
    public abstract bool ShouldRotateNow();

    public virtual bool ForceRotateNow()
    {
        return false;
    }

    public abstract float GetAimTime();

    public abstract float GetCurvedTime(float rawTime);
}
