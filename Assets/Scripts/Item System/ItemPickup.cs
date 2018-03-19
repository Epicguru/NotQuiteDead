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
    }

    public void Update()
    {
        // This is an item, on the floor, that has a collider. Yep.

        if (MouseOver)
        {
            if (AllowPickup && !Item.IsEquipped())
            {
                // Show the user that they can pick this item up.
                if (InputManager.Active)
                {
                    string key = InputManager.GetInput("Pick up").ToString();
                    string itemName = RichText.InBold(RichText.InColour(Item.Name, ItemRarityUtils.GetColour(Item.Rarity)));
                    ActionHUD.DisplayAction("PickupPrompt".Translate(key, itemName));
                }
                if(InputManager.InputDown("Pick up"))
                {
                    // Check client-server situation...
                    if (isServer)
                    {
                        // Pick up manually.
                        Item.RequestDataUpdate();
                        PickupAccepted(Item.Data);
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        // Pick up using the command.
                        Player.Local.NetUtils.CmdRequestPickup(Player.Local.gameObject, this.gameObject);
                    }
                }
            }
        }
    }

    public void PickupAccepted(ItemDataX data)
    {
        PlayerInventory.Add(Item.Prefab, data, 1); // Give real version with data.
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
