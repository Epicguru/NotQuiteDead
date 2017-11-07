using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : NetworkBehaviour
{
    [SyncVar]
    public bool AllowPickup = true;
    public bool MouseOver;
    [HideInInspector] public Item Item;

    public new Collider2D collider;

    public void Start()
    {
        Item = GetComponent<Item>();
        if(collider == null)
            collider = GetComponent<Collider2D>();

        GetComponent<NetPositionSync>().Extrapolate = false;

        collider.isTrigger = true;

        if(GetComponent<Health>() != null)
        {
            GetComponent<Health>().CannotHit.Add(collider);
        }

        Debug.Log("On ground (" + Item.Name + "), data muzzle: " + (Item.Data == null ? "null data" : Item.Data.GUN_Muzzle));
    }

    public void Update()
    {
        // This is an item, on the floor, that has a collider. Yep.

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
                    PlayerInventory.Add(Item.Prefab, Item.Data, 1); // Give real version with data.
                    Debug.Log("Pick up, muzzle is " + Item.Data.GUN_Muzzle);
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
