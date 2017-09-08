﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item))]
public class Throwable : NetworkBehaviour
{

    public GameObject Prefab;

    private Item Item;

    public void Start()
    {
        Item = GetComponent<Item>();
    }

    public void Update()
    {
        if (!Item.IsEquipped())
            return;

        if (InputManager.InputDown("Shoot"))
        {
            // Remove from holding, completely destroy...
            GetComponentInParent<PlayerHolding>().CmdDrop(false, true, Player.Local.gameObject, Item.Data); // Completely destroy this object...

            // Make new object.
            CmdSpawnPrefab(Player.Local.gameObject, transform.position, Quaternion.identity, InputManager.GetMousePos());

            // Look for new throwable of same type in inventory, and if we find one...
            InventoryItem i = PlayerInventory.inv.Inventory.GetOfType(Item.Prefab);
            if (i != null)
            {
                // Equip it!
                Item.Option_Equip(i);
            }
        }
    }

    [Command]
    public void CmdSpawnPrefab(GameObject player, Vector2 position, Quaternion rotation, Vector2 targetPos)
    {
        GameObject x = Instantiate(this.Prefab, position, rotation);
        x.GetComponent<ThrowableInstance>().Target = targetPos;
        x.GetComponent<ThrowableInstance>().StartPosition = position;
        x.transform.position = position;
        NetworkServer.SpawnWithClientAuthority(x, player);
    }
}