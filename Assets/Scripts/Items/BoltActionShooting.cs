using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BoltActionShooting : GunShooting
{
    private Gun gun;

    public override void Start()
    {
        base.Start();

        GetComponent<GunAnimation>().IsRecursive = true;
        gun = GetComponent<Gun>();
    }

    public override void FromAnimReload()
    {
        // Add a bullet
        bulletsInMagazine++;

        if (!CanReload())
        {
            // Gun is full of bullets, and we do not chamber because we are a bolt action rifle that chambers when it closes.
            // Stop the reloading animations.

            gun.Animation.AnimReload(false);
        }
    }
}
