using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class BodyGear : NetworkBehaviour
{
    // Calls when gear is placed on or removed from the player.
    public static UnityEvent GearChangeEvent = new UnityEvent();

    public string Name;
    public Transform GearParent;

    // Currently equipped, is an instance.
    [SyncVar]
    private GameObject ItemGO;

    // The item of the ItemGO, always in sync.
    private Item Item;

    public Item GetItem()
    {
        if (Item == null && ItemGO != null)
        {
            this.Item = ItemGO.GetComponent<Item>();
        }
        if (ItemGO == null)
            Item = null;
        return this.Item;
    }

    [Server]
    public void SetItem(Item item, ItemData data)
    {
        // Item is a PREFAB!

        if(item == null)
        {
            Debug.LogError("Null prefab!");
            return;
        }

        // Make new instance of item...
        Item instance = Item.NewInstance(item.Prefab, transform.position);
        if(data == null)
        {
            data = new ItemData();
        }
        instance.Data = data;
        NetworkServer.Spawn(instance.gameObject);

        SetItem(instance);
    }

    [Server]
    private void SetItem(Item item)
    {
        // Item is an INSTANCE!

        if (item == GetItem())
            return;

        if(item == null)
        {
            DisposeOfOldItem();
        }
        else
        {
            ApplyNewItem(item);
        }

        GearChangeEvent.Invoke();
    }

    private void SetupItem()
    {
        if (ItemGO == null)
            return;

        ItemGO.transform.SetParent(GearParent);
        ItemGO.transform.localPosition = Vector2.zero;
    }

    [Server]
    private void DisposeOfOldItem()
    {
        if (ItemGO == null)
            return;

        // Give item back...
        RpcReturnItem(GetItem().Prefab, GetItem().Data);

        // Destroy the old equipped item.
        Destroy(ItemGO);

        ItemGO = null;
    }

    [Server]
    private void ApplyNewItem(Item item)
    {
        DisposeOfOldItem(); // Does not run if old was null.

        // Set itemGO to this.
        this.ItemGO = item.gameObject;
    }

    [ClientRpc]
    public void RpcReturnItem(string itemPrefab, ItemData data)
    {
        if (data == null)
            data = new ItemData(); // Should not happen.

        PlayerInventory.Add(itemPrefab, data, 1);
    }

    public void Update()
    {
        if (ItemGO != null)
            SetupItem();
    }
}