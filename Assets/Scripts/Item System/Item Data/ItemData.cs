using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    // GENERAL
    public bool Created = false; // Because unity serializes this.

    // GUN
    public int GUN_BulletsInMagazine;
    public bool GUN_BulletInChamber;
    public int GUN_FiringMode;

}
