using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class MeleeDamage
{
    [Tooltip("The base damage per attack of the weapon.")]
    public float Damage = 10;

    [Tooltip("Should this weapon ever be able to damage the local player?")]
    public bool AllowSelfDamage = false;
}
