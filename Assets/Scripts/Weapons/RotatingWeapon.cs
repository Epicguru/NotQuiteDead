using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item))]
public abstract class RotatingItem : NetworkBehaviour
{
    public abstract bool AllowRotateNow();

    public virtual bool ForceRotateNow()
    {
        return false;
    }

    public abstract float GetAimTime();

    public abstract float GetCurvedTime(float rawTime);
}
