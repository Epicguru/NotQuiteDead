using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(GunAiming))]
public class Gun : Weapon
{
    /*
    * Holds information about how a gun behaves and shoots.
    */
    [HideInInspector] public GunAiming Aiming;

    public void Start()
    {
        base.Type = WeaponType.RANGED;
        Aiming = GetComponent<GunAiming>();
    }
}
