using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MiningDrill : NetworkBehaviour
{
    [SyncVar]
    public bool Active;
    public Animator Animator;

    private ItemPickup Pickup;

    public void Start()
    {
        Pickup = GetComponent<ItemPickup>();
    }

    public void Update()
    {
        UpdateActiveState();
        UpdateAnimation();
    }

    public void UpdateActiveState()
    {
        if (Pickup.MouseOver && InputManager.Active)
        {
            ActionHUD.DisplayAction("Press " + InputManager.GetInput("Interact") + " to " + (Active ? "turn off" : "turn on") + " mining drill.");
            if (InputManager.InputDown("Interact"))
            {
                // TODO Network!
                Active = !Active;
            }
        }
    }

    public void UpdateAnimation()
    {
        Animator.SetBool("Active", Active);
    }
}