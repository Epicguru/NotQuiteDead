using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BoltActionShooting : GunShooting
{
    private Gun gun2;

    public override void Start()
    {
        base.Start();

        GetComponent<GunAnimation>().IsRecursive = true;
        gun2 = GetComponent<Gun>();
        Capacity.ChamberCountsAsMag = true; // No chambering.
    }

    public void SetDataDefaults(ItemData data)
    {
        data.GUN_BulletInChamber = true;
        data.GUN_BulletsInMagazine--;
    }

    public override void FromAnimShoot()
    {
        base.FromAnimShoot();

        //if(bulletsInMagazine > 0)
        //    bulletInChamber = true;
    }

    public override void FromAnimReload()
    {
        // Add a bullet...
        if(bulletInChamber) //... IF there is already one in the chamber! 
            bulletsInMagazine++;
        bulletInChamber = true; // After first bullet, chamber is full.

        if (!CanReload())
        {
            // Gun is full of bullets, and we do not chamber because we are a bolt action rifle that chambers when it closes.
            // Stop the reloading animations.

            gun2.Animation.AnimReload(false);
        }
    }
}
