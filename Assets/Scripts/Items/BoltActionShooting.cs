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

    public void SetDataDefaults(ItemDataX data)
    {
        data.Update("Bullet In Chamber", true);
        if(data.ContainsKey("Bullets In Magazine"))
        {
            int bullets = data.Get<int>("Bullets In Magazine");
            data.Update("Bullets In Magazine", --bullets);
        }
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
        if(BulletInChamber) //... IF there is already one in the chamber! 
            bulletsInMagazine++;
        BulletInChamber = true; // After first bullet, chamber is full.

        if (!CanReload())
        {
            // Gun is full of bullets, and we do not chamber because we are a bolt action rifle that chambers when it closes.
            // Stop the reloading animations.

            gun2.Animation.AnimReload(false);
        }
    }
}
