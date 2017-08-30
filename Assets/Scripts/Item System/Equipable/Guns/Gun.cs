using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(GunAiming), typeof(GunAnimation), typeof(GunShooting))]
public class Gun : Weapon
{
    /*
    * Holds information about how a gun behaves and shoots.
    */

    [HideInInspector] public GunAiming Aiming;
    [HideInInspector] public GunAnimation Animation;
    [HideInInspector] public GunShooting Shooting;
    [HideInInspector] public Item Item;

    public void Start()
    {
        base.Type = WeaponType.RANGED;

        Aiming = GetComponent<GunAiming>();
        Animation = GetComponent<GunAnimation>();
        Shooting = GetComponent<GunShooting>();
        Item = GetComponent<Item>();

        if(GetComponentInChildren<GunAnimationCallbacks>() == null)
        {
            Debug.LogError("No gun animation callback object on children. It should be on the same object as the Animator.");
            Debug.Break();
        }
    }

    public void Update()
    {
        Animation.AnimRun(Item.IsEquipped() && InputManager.InputPressed("Sprint"));
    }
}
