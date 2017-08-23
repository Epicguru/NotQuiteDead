using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PolygonCollider2D), typeof(Rigidbody2D))]
public class ItemPickup : NetworkBehaviour
{
    public bool AllowPickup = true;
    [HideInInspector] public bool MouseOver;
    [HideInInspector] public Item Item;

    private new PolygonCollider2D collider;
    private new Rigidbody2D rigidbody;

    public void Start()
    {
        Item = GetComponent<Item>();
        collider = GetComponent<PolygonCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();

        rigidbody.drag = 3f;
        rigidbody.angularDrag = 2f;
        rigidbody.useAutoMass = true; // Meh.

        GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncRigidbody2D;

        collider.isTrigger = true;
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
                if(InputManager.InputDown("Pick up"))
                {
                    // Set player authority
                    if(!GetComponent<NetworkIdentity>().hasAuthority)
                        Player.Local.RequestAuthority(this.gameObject);
                    

                    // Pick up!
                    PlayerInventory.Add(Item.FindItem(Item.Prefab)); // Give prefab version.
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
