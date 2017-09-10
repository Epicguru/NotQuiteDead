using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class ItemData
{

    // Unity networking does not understand inheritance, so this is my temporary solution to tranferring and saving item data.

    // GENERAL
    public bool Created = false; // Because unity serializes this.

    // GUN
    public int GUN_BulletsInMagazine;
    public bool GUN_BulletInChamber;
    public int GUN_FiringMode;
    public string GUN_Muzzle;
    public string GUN_Sight;
    public string GUN_Magazine;
    public string GUN_UnderBarrel;

}
