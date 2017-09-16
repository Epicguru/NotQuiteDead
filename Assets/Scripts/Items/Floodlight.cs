using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Floodlight : NetworkBehaviour {

    [SyncVar]
    public bool Active;

    public Light Light;

    private ItemPickup pickup;
    private Placeable placeable;

    public void Start()
    {
        pickup = GetComponent<ItemPickup>();
        placeable = GetComponent<Placeable>();
    }

    public void Update()
    {
        if (placeable.IsPlaced && pickup.MouseOver && InputManager.Active)
        {
            ActionHUD.DisplayAction("Press " + InputManager.GetInput("Interact") + " to " + (Active ? "turn off" : "turn on") + ".");
            if (InputManager.InputDown("Interact"))
            {
                Player.Local.NetUtils.CmdToggleFloodlight(gameObject, !Active);
            }
        }

        if (!Active || !placeable.IsPlaced)
        {
            // Stop light...
            Light.enabled = false;
        }
        else
        {
            Light.enabled = true;
        }
    }
}
