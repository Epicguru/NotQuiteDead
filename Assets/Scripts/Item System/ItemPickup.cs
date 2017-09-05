using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PolygonCollider2D), typeof(Rigidbody2D))]
public class ItemPickup : NetworkBehaviour
{
    [SyncVar]
    public bool AllowPickup = true;
    public bool MouseOver;
    [HideInInspector] public Item Item;

    public new PolygonCollider2D collider;
    private new Rigidbody2D rigidbody;

    public void Start()
    {
        Item = GetComponent<Item>();
        if(collider == null)
            collider = GetComponent<PolygonCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();

        rigidbody.drag = 3f;
        rigidbody.angularDrag = 2f;
        rigidbody.useAutoMass = true; // Meh.

        GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncRigidbody2D;

        collider.isTrigger = true;

        if(GetComponent<Health>() != null)
        {
            GetComponent<Health>().CannotHit.Add(collider);
        }
    }

    public void Update()
    {
        // This is an item, on the floor, that has a collider. Yep.

        // Set values for rigidbody.
        rigidbody.gravityScale = 0; // Float, because we are a top down map.
        if (!Item.IsEquipped())
        {
            // Activate rigidbody.
            rigidbody.isKinematic = false;
        }
        else
        {
            rigidbody.isKinematic = true;
        }

        if (MouseOver)
        {
            if (AllowPickup && !Item.IsEquipped())
            {
                // Show the user that they can pick this item up.
                if(InputManager.Active)
                    ActionHUD.DisplayAction("Press " + InputManager.GetInput("Pick up") + " to pick up " + RichText.InBold(RichText.InColour(Item.Name, ItemRarityUtils.GetColour(Item.Rarity))) + ".");
                if(InputManager.InputDown("Pick up"))
                {
                    // Set player authority : EDIT - Not needed.
                    //if(!GetComponent<NetworkIdentity>().hasAuthority)
                    //    Player.Local.RequestAuthority(this.gameObject);


                    // Pick up!
                    Item.RequestDataUpdate(); // Update data.
                    PlayerInventory.Add(Item); // Give real version with data.
                    Player.Local.NetUtils.CmdDestroyItem(this.gameObject);
                }
            }
        }
    }

    public void OnMouseEnter()
    {
        MouseOver = true;
    }

    public void OnMouseExit()
    {
        MouseOver = false;
    }
}
