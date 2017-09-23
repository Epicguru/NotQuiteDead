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
        Bow = GetComponent<Bow>();
    }

    public void FireArrow()
    {

    }

    public void EndFire()
    {

    }
}