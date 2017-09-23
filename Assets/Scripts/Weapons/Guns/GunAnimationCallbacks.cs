using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GunAnimationCallbacks : ItemAnimationCallback
{
    private Gun gun;

    private void Start()
    {
        gun = GetComponentInParent<Gun>();
    }

    public void ReloadEnd()
    {
        if(Active)
            gun.Animation.CallbackReloadEnd();
    }

    public void ChamberEnd()
    {
        if (Active)
            gun.Animation.CallbackChamberEnd();
    }

    public void Shoot()
    {
        if (Active)
            gun.Animation.CallbackShoot();
    }

    public void EquipEnd()
    {
        if (Active)
            gun.Animation.CallbackEquipEnd();
    }

    public void SpawnShell()
    {
        if (Active)
            gun.Animation.CallbackSpawnShell();
    }

    public void SpawnMag()
    {
        if (Active)
            gun.Animation.CallbackSpawnMag();
    }
}
