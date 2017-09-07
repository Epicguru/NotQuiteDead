using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class GunCapacity
{
    /*
    * Holds information about the capacity of the weapon in terms of bullets/ammo.
    */

    [Tooltip("The amount of ammo that can fit in one magazine of the gun.")]
    public int MagazineCapacity = 10;
    [Tooltip("The min/max amount of bullets/pellets per shot of the gun. Useful for shotguns and other spread weapons. Both values are inclusive in random selection.")]
    public Vector2 BulletsPerShot = new Vector2(1, 1);
    [Tooltip("The amount of ammo consumed per shot.")]
    public int BulletsConsumed = 1;
    [Tooltip("If true, then the bullet stored in the chamber will be considered when allowing reloading. Useful for single shot weapons.")]
    public bool ChamberCountsAsMag = false;
    [Tooltip("The amount of shots per burst fire. Only applies if the gun has burst fire firing mode active.")]
    public int BurstShots = 3;
}
