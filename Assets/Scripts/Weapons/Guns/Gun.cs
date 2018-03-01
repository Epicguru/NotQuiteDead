using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[RequireComponent(typeof(GunAiming), typeof(GunAnimation), typeof(GunShooting))]
[RequireComponent(typeof(GunAttachments))]
public class Gun : Weapon
{
    /*
    * Holds information about how a gun behaves and shoots.
    */

    public GunType GunType = GunType.RIFLE;

    public UnityEvent OnShoot = new UnityEvent();

    public GunAiming Aiming
    {
        get
        {
            if (_Aiming == null)
                _Aiming = GetComponent<GunAiming>();
            return _Aiming;
        }
    }
    private GunAiming _Aiming;
    public GunAnimation Animation
    {
        get
        {
            if (_Animation == null)
                _Animation = GetComponent<GunAnimation>();
            return _Animation;
        }
    }
    private GunAnimation _Animation;
    public GunShooting Shooting
    {
        get
        {
            if (_Shooting == null)
                _Shooting = GetComponent<GunShooting>();
            return _Shooting;
        }
    }
    private GunShooting _Shooting;
    public Item Item
    {
        get
        {
            if (_Item == null)
                _Item= GetComponent<Item>();
            return _Item;
        }
    }
    private Item _Item;
    public GunAttachments Attachments
    {
        get
        {
            if (_Attachments == null)
                _Attachments = GetComponent<GunAttachments>();
            return _Attachments;
        }
    }
    private GunAttachments _Attachments;

    public void Start()
    {
        base.Type = WeaponType.RANGED;

        if(GetComponentInChildren<GunAnimationCallbacks>() == null)
        {
            Debug.LogError("No gun animation callback object on children. It should be on the same object as the Animator.");
            Debug.Break();
        }        
    }

    public void Update()
    {
        if(isClient && hasAuthority)
            Animation.AnimRun(Item.IsEquipped() && InputManager.InputPressed("Sprint") && Player.Local.GetComponent<Rigidbody2D>().velocity != Vector2.zero);

        transform.localRotation = Quaternion.identity;
    }

    public void UpdateData(ItemDataX data)
    {
        data.Update("Bullet In Chamber", Shooting.BulletInChamber);
        data.Update("Bullets In Magazine", Shooting.bulletsInMagazine);
        data.Update("Firing Mode", Shooting.firingModeIndex);
    }

    public void ApplyData(ItemDataX data)
    {
        Shooting.BulletInChamber = data.Get("Bullet In Chamber", true);
        Shooting.bulletsInMagazine = data.Get("Bullets In Magazine", 0);
        Shooting.firingModeIndex = data.Get("Firing Mode", 0);
    }

    public void SetDataDefaults(ItemDataX data)
    {
        // Set gun data.
        data.Update("Bullet In Chamber", Shooting.ReloadAutoChambers);
        data.Update("Bullets In Magazine", Shooting.Capacity.MagazineCapacity - (Shooting.ReloadAutoChambers ? 1 : 0));
        data.Update("Firing Mode", 0);
    }
}
