using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item))]
public class ItemGear : NetworkBehaviour
{

    // Represents the item object of a type of gear that is placed on the player's body.
    // Has stats such as armour, HP boost, speed penalty, warmth ect.

    [Tooltip("The slot that this equips onto.")]
    public string Slot;

    [Tooltip("A multiplier that affects player movement speed. The value is set as in:\n* 0.3 means the player moves 30% faster,\n* -0.5 means the player moves 50% slower.")]
    public float SpeedChange;

    [Tooltip("A multiplier that affects total player health. The value is set as in:\n* 0.3 means the player has 30% more health,\n* -0.5 means the player has 50% less health.")]
    public float HealthChange;

    public bool Equipped
    {
        get
        {
            return transform.parent != null;
        }
    }

    public void Update()
    {
        // Note that the parent is set automatically from the player gear script.
    }
}