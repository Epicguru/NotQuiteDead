using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PolygonCollider2D))]
public class ItemPickup : NetworkBehaviour
{
    public bool AllowPickup = true;

    public bool MouseOver;
    private new PolygonCollider2D collider;
    [HideInInspector] public Item Item;

    public void Start()
    {
        Item = GetComponent<Item>();
        collider = GetComponent<PolygonCollider2D>();

        collider.isTrigger = true;
    }

    public void Update()
    {
        // This is an item, on the floor, that has a collider. Yep.

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
