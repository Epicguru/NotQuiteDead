using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BodyGearStats : NetworkBehaviour
{
    // Handles the currently equipped gear stats.

    public Player Player;

    public void Start()
    {
        Player = GetComponent<Player>();
        BodyGear.GearChangeEvent.AddListener(GearChanged);
    }

    [Server]
    public void GearChanged()
    {
        if (!isServer) // Not necessary
        {
            Debug.LogError("Gear changed called on client!");
            return;
        }


    }
}