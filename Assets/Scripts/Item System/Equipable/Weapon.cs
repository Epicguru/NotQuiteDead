using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item))]
public abstract class Weapon : NetworkBehaviour
{
    // Some very general information about an item that is considered a weapon.

    [Tooltip("The type of weapon, such as Melee or Ranged. This may be set automatically by other scripts.")]
    public WeaponType Type;
}