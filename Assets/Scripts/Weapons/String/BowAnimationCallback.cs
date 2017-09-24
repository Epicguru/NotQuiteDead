using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BowAnimationCallback : ItemAnimationCallback
{
    [HideInInspector]
    public Bow Bow;

    public void Start()
    {
        Bow = GetComponentInParent<Bow>();
    }

    public void FireArrow()
    {
        Bow.CallbackArrowFire();
    }

    public void EndFire()
    {
        Bow.CallbackFireEnd();
    }
}