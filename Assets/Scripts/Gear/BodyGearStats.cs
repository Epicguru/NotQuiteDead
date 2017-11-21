using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BodyGearStats : NetworkBehaviour
{
    // Handles the currently equipped gear stats.

    public Player Player;

    public float HealthMulti;
    public float SpeedMulti;

    public void Awake()
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

        //Debug.Log("Gear changed!");

        // Reset all stats.
        HealthMulti = 1f;
        SpeedMulti = 1f;

        // Calculate all stats again.
        foreach(BodyGear bg in Player.BodyGear)
        {
            if (bg.GetGearItem() != null)
            {
                float h = bg.GetGearItem().HealthChange;
                float s = bg.GetGearItem().SpeedChange;
                HealthMulti += h;
                SpeedMulti += s;
            }
        }
    }
}