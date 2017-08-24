using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GunAnimationCallbacks : MonoBehaviour
{
    private Gun gun;

    public void Start()
    {
        gun = GetComponentInParent<Gun>();
    }

    public void ReloadEnd()
    {
        gun.Animaiton.CallbackReloadEnd();
    }

    public void ChamberEnd()
    {
        gun.Animaiton.CallbackChamberEnd();
    }

    public void Shoot()
    {
        gun.Animaiton.CallbackShoot();
    }

    public void EquipEnd()
    {

    }
}
