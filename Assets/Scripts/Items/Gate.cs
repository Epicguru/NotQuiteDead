using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Placeable))]
public class Gate : NetworkBehaviour
{
    [SyncVar]
    public bool IsOpen;
    public Animator Animator;
    public string OpenBool = "Open";

    private ItemPickup pickup;
    private Item item;
    private Placeable placeable;

    public void Start()
    {
        pickup = GetComponent<ItemPickup>();
        item = GetComponent<Item>();
        placeable = GetComponent<Placeable>();
    }

    public void Update()
    {
        // For now, anyone can open...
        if (pickup.MouseOver && placeable.IsPlaced)
        {
            // Display to user
            ActionHUD.DisplayAction("Press " + InputManager.GetInput("Interact") + " to " + (IsOpen ? "close" : "open") + " the gate.");
            if (InputManager.InputDown("Interact"))
            {
                Player.Local.NetUtils.CmdOpenGate(gameObject, !IsOpen);
            }
        }

        Animator.SetBool(OpenBool, IsOpen);
    }
}
