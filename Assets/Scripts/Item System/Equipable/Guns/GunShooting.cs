using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GunShooting : NetworkBehaviour
{
    public FiringMode FiringMode;

    private Gun gun;
    private new GunAnimation animation;

    public void Start()
    {
        gun = GetComponent<Gun>();
        animation = GetComponent<GunAnimation>();
    }

    public void Update()
    {
        // Shooting...
        if (!hasAuthority)
            return; // Owner only...

        bool requestingShoot = ShootNow();
        bool ready = animation.IsAiming && !animation.IsDropped && !animation.IsChambering && !animation.IsReloading;
        animation.AnimShoot(requestingShoot && ready); // Animation, bullets and damage later.

        bool requestingReload = InputManager.InputDown("Reload");
        ready = !animation.IsReloading && !animation.IsAiming && !animation.IsChambering && !animation.IsDropped;

        if(requestingReload && ready)
        {
            // Reload!
            animation.AnimReload();
        }
    }

    public bool ShootNow()
    {
        // Returns true if the gun SHOULD shoot.Whether the gun will shoot is up to other factors.

        switch (FiringMode)
        {
            case FiringMode.SEMI:
                // Shoot only when the shoot button is first pressed.
                if (InputManager.InputDown("Shoot"))
                    return true;
                break;
            case FiringMode.AUTO:
                // Shoot forever if the shoot button is pressed.
                if (InputManager.InputPressed("Shoot"))
                    return true;
                break;
            case FiringMode.BURST:
                // TODO implement me.
                break;
        }

        return false;
    }

}
