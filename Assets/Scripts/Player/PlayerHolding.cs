using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// A class that handles the equiping of item on the player.

// An item object, which exists only on the ground or equiped, can be equiped by the player and is then
// held in the players hands.

public class PlayerHolding : NetworkBehaviour
{
    public Transform Holding; // The game obejct that holds items that are currenty equiped.
    public Item Item; // The currently equipped item.
    private HandyRemoval handyRemoval;

    public void Start()
    {
        handyRemoval = GetComponent<HandyRemoval>();
    }

    [Command]
    public void CmdEquip(string prefab, GameObject localPlayer)
    {
        // We need to equip this item.
        // This runs on the server, which we can think of as in the middle of nowhere.
        // We need to create the item here, but do net mess with settings because that will
        // not transmit to clients.

        // Checks
        if (string.IsNullOrEmpty(prefab))
        {
            Debug.LogError("Prefab string was null or empty.");
            return;
        }

        Item created = Item.NewInstance(prefab);

        // Assuming that this item has not been spawned...
        NetworkServer.SpawnWithClientAuthority(created.gameObject, localPlayer);

        // Setting the item state to equipped sets the parent automatically.

        // Set states
        created.SetEquipped(true, localPlayer);
        created.SetDropped(false);

        RpcSetItem(created.gameObject, localPlayer);

        Debug.Log("Created item!");
    }

    [ClientRpc]
    public void RpcSetItem(GameObject item, GameObject player)
    {
        this.Item = item.GetComponent<Item>();
    }

    // TODO network!
    public void Drop()
    {
        // Drops the currently held item, if holding anything.
        // TODO
    }

    public void Update()
    {
        if(isLocalPlayer)
            handyRemoval.CmdSetShowHands(Item == null);
    }
}