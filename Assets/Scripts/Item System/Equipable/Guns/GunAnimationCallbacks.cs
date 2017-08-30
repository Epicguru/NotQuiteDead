using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GunAnimationCallbacks : ItemAnimationCallback
{
    private Gun gun;

    public void Start()
    {
        gun = GetComponentInParent<Gun>();
    }

    public void ReloadEnd()
    {
        gun.Animation.CallbackReloadEnd();
    }

    public void ChamberEnd()
    {
        gun.Animation.CallbackChamberEnd();
    }

    public void Shoot()
    {
        gun.Animation.CallbackShoot();
    }

    public void EquipEnd()
    {
        gun.Animation.CallbackEquipEnd();
    }
}
