using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(GunAiming), typeof(GunAnimation), typeof(GunShooting))]
[RequireComponent(typeof(GunAttachments))]
public class Gun : Weapon
{
    /*
    * Holds information about how a gun behaves and shoots.
    */

    public GunType GunType = GunType.RIFLE;

    [HideInInspector] public GunAiming Aiming;
    [HideInInspector] public GunAnimation Animation;
    [HideInInspector] public GunShooting Shooting;
    [HideInInspector] public Item Item;
    [HideInInspector] public GunAttachments Attachments;

    public void Start()
    {
        base.Type = WeaponType.RANGED;

        Aiming = GetComponent<GunAiming>();
        Animation = GetComponent<GunAnimation>();
        Shooting = GetComponent<GunShooting>();
        Item = GetComponent<Item>();
        Attachments = GetComponent<GunAttachments>();

        if(GetComponentInChildren<GunAnimationCallbacks>() == null)
        {
            Debug.LogError("No gun animation callback object on children. It should be on the same object as the Animator.");
            Debug.Break();
        }
    }

    public void Update()
    {
        if(isClient && hasAuthority)
            Animation.AnimRun(Item.IsEquipped() && InputManager.InputPressed("Sprint"));
    }

    public void UpdateData(ItemData data)
    {
        Shooting = GetComponent<GunShooting>();
        data.GUN_BulletInChamber = Shooting.bulletInChamber;
        data.GUN_BulletsInMagazine = Shooting.bulletsInMagazine;
        data.GUN_FiringMode = Shooting.firingModeIndex;
    }

    public void ApplyData(ItemData data)
    {
        Shooting = GetComponent<GunShooting>();
        Shooting.bulletInChamber = data.GUN_BulletInChamber;
        Shooting.bulletsInMagazine = data.GUN_BulletsInMagazine;
        Shooting.firingModeIndex = data.GUN_FiringMode;
    }

    public void SetDataDefaults(ItemData data)
    {
        // Set gun data.
        Shooting = GetComponent<GunShooting>();
        data.GUN_BulletInChamber = false;
        data.GUN_BulletsInMagazine = Shooting.Capacity.MagazineCapacity;
        data.GUN_FiringMode = 0;
    }
}
