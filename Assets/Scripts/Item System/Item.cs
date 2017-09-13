﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[RequireComponent(typeof(ItemPickup), typeof(NetworkTransform), typeof(SpriteLighting))]
public class Item : NetworkBehaviour
{

    /*
    * Represents an item that can be found, crafted, dropped etc.
    * This is attached to the actual game object that is the item. Items can be stored in inventories.
    * Some items are equipable, but the equping actions will be handled in another script.
    */

    [Tooltip("The unique ID of this item. Used to spawn new items.")]
    public string Prefab = "Prefab Name";

    [Tooltip("The display name of this item.")]
    public string Name = "Default Item Name";

    [Tooltip("The rarity (tier) of the item.")]
    public ItemRarity Rarity;

    [Tooltip("Whether the item can stack with other items in the inventory.")]
    public bool CanStack = false;

    /// <summary>
    /// Whether the item can be equipped.
    /// The equipped actions are handled in another script.
    /// </summary>
    [Tooltip("Whether the item can be equipped into the players hands.")]
    public bool Equipable = false;

    [Tooltip("Descriptions of the item.")]
    public ItemDescription Description;

    [Tooltip("Information about the space and weight that this item takes in an inventory.")]
    public InventoryInfo InventoryInfo;

    [Tooltip("The image for the icon in menus and inventories.")]
    public Sprite ItemIcon;

    [Tooltip("Current item data.")]
    [SyncVar]
    [HideInInspector]
    public ItemData Data;

    [SyncVar]
    private bool equiped = false;
    private NetworkTransform netTransform;
    [SyncVar]
    private GameObject PlayerHolding;
    [HideInInspector] public ItemPickup pickup;
    [HideInInspector] public SpriteLighting Lighting;
    private string currentLayer;

    public void Start()
    {
        netTransform = GetComponent<NetworkTransform>();
        pickup = GetComponent<ItemPickup>();
        Lighting = GetComponent<SpriteLighting>();

        if (Data == null || Data.Created == false)
        {
            if (Data == null)
                Data = new ItemData();
            Data.Created = true;
            RequestSetDefaultData();
            RequestDataApplication(); // Apply loaded or transmitted data.
        }
        else
        {
            RequestDataApplication(); // Apply loaded or transmitted data.
        }
    }

    public void SetLayer(string layer)
    {
        if (currentLayer == layer)
            return;
        foreach(SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
        {
            r.sortingLayerName = layer;
        }
        currentLayer = layer;
    }

    public ItemOption[] CreateOptions(ItemData data)
    {
        List<ItemOption> options = new List<ItemOption>();
        options.Add(new ItemOption() { OptionName = "Drop", OnSelected = Option_Drop });
        if(Equipable)
            options.Add(new ItemOption() { OptionName = "Equip", OnSelected = Option_Equip });
        options.Add(new ItemOption() { OptionName = "Details", OnSelected = Option_Details });
        if (GetComponent<Attachment>() != null && Player.Local.Holding.Item != null && Player.Local.Holding.Item.GetComponent<GunAttachments>() != null)
            options.Add(new ItemOption() { OptionName = "Put On Current Weapon", OnSelected = Option_ApplyAttachment });
        if (!string.IsNullOrEmpty(data.GUN_Magazine))
        {
            options.Add(new ItemOption() { OptionName = "Detach " + data.GUN_Magazine, OnSelected = Option_RemoveMagazine });
        }
        if (!string.IsNullOrEmpty(data.GUN_Muzzle))
        {
            options.Add(new ItemOption() { OptionName = "Detach " + data.GUN_Muzzle, OnSelected = Option_RemoveMuzzle });
        }
        if (!string.IsNullOrEmpty(data.GUN_Sight))
        {
            options.Add(new ItemOption() { OptionName = "Detach " + data.GUN_Sight, OnSelected = Option_RemoveSight });
        }
        if (!string.IsNullOrEmpty(data.GUN_UnderBarrel))
        {
            options.Add(new ItemOption() { OptionName = "Detach " + data.GUN_UnderBarrel, OnSelected = Option_RemoveUnderBarrel });
        }

        return options.ToArray();
    }

    public void Update()
    {
        // Does not matter where this is running...
        netTransform.enabled = (transform.parent == null);

        // Layer
        SetLayer(IsEquipped() ? "Equipped Items" : "Dropped Items");

        UpdateParent();
    }

    public void RequestDataUpdate()
    {
        // Indicates that we should get Data up-to-date. Happens when the item changes state.
        this.BroadcastMessage("UpdateData", Data, SendMessageOptions.DontRequireReceiver);
    }

    public void RequestDataApplication()
    {
        // Indicates that we should apply the data. Happens when the item changes state.
        this.BroadcastMessage("ApplyData", Data, SendMessageOptions.DontRequireReceiver);
    }

    public void RequestSetDefaultData()
    {
        // Called when item spawns out of nowhere, such as a random spawn event or a mob drop.
        this.BroadcastMessage("SetDataDefaults", Data, SendMessageOptions.DontRequireReceiver);
    }

    public void UpdateParent()
    {
        if (IsEquipped())
        {
            transform.SetParent(PlayerHolding.GetComponent<Player>().Holding.Holding);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.SetParent(null);
        }
    }

    /// <summary>
    /// Gets the Equipped state. More handling for equippable items in another script. TODO.
    /// </summary>
    /// <returns>True if equipped.</returns>
    public bool IsEquipped()
    {
        return this.equiped;
    }

    /// <summary>
    /// Sets the eqipped state. If the state is already active, may it be true or false,
    /// this call will be ignored.
    /// </summary>
    public void SetEquipped(bool equipped, GameObject player)
    {
        if (equipped)
        {
            if (!this.equiped)
            {
                // Activate equipped state.
                if (player == null)
                    Debug.LogError("Player to hold this item is null!");
                this.equiped = true;
                this.PlayerHolding = player;

                //Debug.Log("Item '" + Name + "' equipped to '" + player.name + "'");
            }
        }
        else
        {
            if (this.equiped)
            {
                // Remove equipped state.
                this.PlayerHolding = null;
                this.equiped = false;
            }
        }
    }

    public static Dictionary<string, Item> Items;

    public static void LoadItems()
    {
        Items = new Dictionary<string, Item>();
        Object[] items = Resources.LoadAll("Items/", typeof(Item));

        foreach(Object o in items)
        {
            Item x = (Item)o;
            Items.Add(x.Prefab, (Item)o);
            Debug.Log("Loaded item '" + x.Prefab + "'");
        }
    }

    public static void RegisterItems()
    {
        if (Items == null)
        {
            Debug.LogError("Items is null!");
            return;
        }

        foreach(Item i in Items.Values)
        {
            NetworkManager.singleton.spawnPrefabs.Add(i.gameObject);
        }

        foreach (ThrowableInstance t in Resources.LoadAll<ThrowableInstance>("Items/"))
        {
            NetworkManager.singleton.spawnPrefabs.Add(t.gameObject);
        }
    }

    /// <summary>
    /// Creates a new instance of an object and spawns it into the world.
    /// </summary>
    public static Item NewInstance(string prefab)
    {
        // Create new instance of item.
        Item x = FindItem(prefab);
        // TODO DATA!
        Item newItem = Instantiate(x, Vector3.zero, Quaternion.identity);

        return newItem;
    }

    /// <summary>
    /// Gets an item from the loaded items, this does not create a new instance.
    /// </summary>
    /// <param name="path">The prefab name of the item, as in Item.Prefab .</param>
    /// <returns>The item object, which is a prefab.</returns>
    public static Item FindItem(string prefab)
    {
        if (Items == null)
            LoadItems();
        if (!Items.ContainsKey(prefab))
        {
            Debug.LogError("Item not found! '" + prefab + "'");
            return null;
        }
        return Items[prefab];
    }

    public static void Option_Equip(InventoryItem x)
    {
        ItemData d = x.Data;
        if(d == null)
        {
            d = new ItemData();
        }
        x.Data = d;
        x.Inventory.RemoveItem(x, Vector2.zero, false); // Remove, do not drop.
        Player.Local.Holding.CmdEquip(x.Item.Prefab, Player.Local.gameObject, d);
    }

    public static void Option_Drop(InventoryItem x)
    {
        x.Inventory.RemoveItem(x, Player.Local.transform.position, true);
    }

    public static void Option_Details(InventoryItem x)
    {
        x.Inventory.DetailsView.Enter(x);
    }

    public static void Option_ApplyAttachment(InventoryItem x)
    {
        // Set attachment...
        bool worked = Player.Local.Holding.Item.GetComponent<GunAttachments>().SetAttachment(x.Item.GetComponent<Attachment>().Type, x.Item);
        // Remove from inventory
        if(worked)
            x.Inventory.RemoveItem(x, Vector2.zero, false, 1);
    }

    public static void Option_RemoveMagazine(InventoryItem x)
    {
        // Remove attachment...
        string old = x.Data.GUN_Magazine;
        x.Data.GUN_Magazine = null;

        // Give item back to player!
        x.Inventory.AddItem(Item.FindItem(old), 1);
    }

    public static void Option_RemoveMuzzle(InventoryItem x)
    {
        // Remove attachment...
        string old = x.Data.GUN_Muzzle;
        x.Data.GUN_Muzzle = null;

        // Give item back to player!
        x.Inventory.AddItem(Item.FindItem(old), 1);
    }

    public static void Option_RemoveSight(InventoryItem x)
    {
        // Remove attachment...
        string old = x.Data.GUN_Sight;
        x.Data.GUN_Sight = null;

        // Give item back to player!
        x.Inventory.AddItem(Item.FindItem(old), 1);
    }

    public static void Option_RemoveUnderBarrel(InventoryItem x)
    {
        // Remove attachment...
        string old = x.Data.GUN_UnderBarrel;
        x.Data.GUN_UnderBarrel = null;

        // Give item back to player!
        x.Inventory.AddItem(Item.FindItem(old), 1);
    }
}