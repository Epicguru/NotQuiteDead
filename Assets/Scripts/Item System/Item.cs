using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[RequireComponent(typeof(ItemPickup), typeof(NetPositionSync))]
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

    [Tooltip("If true AND Equipable is true, the this item can be quick-slotted.")]
    public bool CanQuickSlot = true;

    [Tooltip("Descriptions of the item.")]
    public ItemDescription Description;

    [Tooltip("Information about the space and weight that this item takes in an inventory.")]
    public InventoryInfo InventoryInfo;

    [Tooltip("The image for the icon in menus and inventories.")]
    public Sprite ItemIcon;

    public ItemDataX Data
    {
        get
        {
            return _Data;
        }
        set
        {
            if (IsPrefab)
            {
                Debug.LogError("Cannot set item data for an item prefab!");
                Debug.Log(value);
                return;
            }
            _Data = value;
        }
    }

    [Tooltip("Current item data.")]
    [SerializeField]
    private ItemDataX _Data;

    [HideInInspector]
    public NetPositionSync NetPosSync;

    [SyncVar]
    [SerializeField]
    private bool equipped = false;
    [SyncVar]
    private GameObject PlayerHolding;
    [HideInInspector] public ItemPickup Pickup;
    private string currentLayer;

    private bool IsPrefab = true;
    private bool IsGear = false;

    public void Awake()
    {
        IsPrefab = false;
        IsGear = GetComponent<GearItem>() != null;
        NetPosSync = GetComponent<NetPositionSync>();
        Pickup = GetComponent<ItemPickup>();
    }

    public void Start()
    {
        if (Data == null || Data.Created == false)
        {
            if (Data == null)
                Data = new ItemDataX();
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
        foreach(SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>(true))
        {
            if(r.gameObject.layer != 9)
                r.sortingLayerName = layer;
        }
        currentLayer = layer;
    }

    public ItemOption[] CreateOptions(Item item, ItemDataX data)
    {
        List<ItemOption> options = new List<ItemOption>();
        options.Add(new ItemOption() { OptionName = "Drop", OnSelected = Option_Drop });
        if(Equipable && item.GetComponent<GearItem>() == null && !Player.Local.Building.InBuildMode)
            options.Add(new ItemOption() { OptionName = "Equip", OnSelected = Option_Equip });
        options.Add(new ItemOption() { OptionName = "Details", OnSelected = Option_Details });
        if (GetComponent<Attachment>() != null && Player.Local.Holding.Item != null && Player.Local.Holding.Item.GetComponent<GunAttachments>() != null && Player.Local.Holding.Item.GetComponent<GunAttachments>().IsValid(item.GetComponent<Attachment>().Type, item.GetComponent<Attachment>()))
            options.Add(new ItemOption() { OptionName = "Put On Current Weapon", OnSelected = Option_ApplyAttachment });
        if (item.GetComponent<GearItem>() != null)
            options.Add(new ItemOption() { OptionName = "Equip", OnSelected = Option_EquipGear });
        if(item.Equipable)
            options.Add(new ItemOption() { OptionName = "Quick Slot...", OnSelected = Option_QuickSlot });

        if (data == null)
            return options.ToArray();

        if (!string.IsNullOrEmpty(data.Get<string>("Magazine Attachment")))
        {
            options.Add(new ItemOption() { OptionName = "Detach " + NameOf(data.Get<string>("Magazine Attachment")), OnSelected = Option_RemoveMagazine });
        }
        if (!string.IsNullOrEmpty(data.Get<string>("Muzzle Attachment")))
        {
            options.Add(new ItemOption() { OptionName = "Detach " + NameOf(data.Get<string>("Muzzle Attachment")), OnSelected = Option_RemoveMuzzle });
        }
        if (!string.IsNullOrEmpty(data.Get<string>("Sight Attachment")))
        {
            options.Add(new ItemOption() { OptionName = "Detach " + NameOf(data.Get<string>("Sight Attachment")), OnSelected = Option_RemoveSight });
        }
        if (!string.IsNullOrEmpty(data.Get<string>("Under Barrel Attachment")))
        {
            options.Add(new ItemOption() { OptionName = "Detach " + NameOf(data.Get<string>("Under Barrel Attachment")), OnSelected = Option_RemoveUnderBarrel });
        }

        return options.ToArray();
    }

    public void Update()
    {
        // Does not matter where this is running...
        NetPosSync.enabled = (transform.parent == null);

        // Layer
        SetLayer(IsEquipped() ? "Equipped Items" : "Dropped Items");

        UpdateParent();
    }

    public virtual bool ShouldSerialize()
    {
        // Should this item be serialized? Generally should only return true if it is dropped on the ground.

        // Do not serialize equipped items.
        if (IsEquipped())
        {
            return false;
        }

        // Do not serialize gear items if they are equipped.
        GearItem gear = GetComponent<GearItem>();
        if(gear != null)
        {
            if (gear.IsEquipped)
            {
                return false;
            }
        }

        // Do not serialize attachments if they are currently attached to a weapon.
        Attachment a = GetComponent<Attachment>();
        if (a != null && a.IsAttached)
        {
            return false;
        }

        return true;
    }

    public virtual void LoadSaveData(ItemSaveData saveData)
    {
        // Used for custom data that may need to be saved for particular types of items.
        // Called only on the server just after the item has been spawned.
        return;
    }

    public virtual ItemSaveData GetSaveData()
    {
        return new ItemSaveData(this);
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
        Debug.Log("Applied data! (" + Prefab + ")");

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
            Transform t = PlayerHolding.GetComponent<Player>().Holding.Holding;
            if (transform.parent != t)
                transform.SetParent(t);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }
        else
        {
            if (!IsGear)
                if (transform.parent != null)
                    transform.SetParent(null);
        }
    }

    /// <summary>
    /// Gets the Equipped state. More handling for equippable items in another script. TODO.
    /// </summary>
    /// <returns>True if equipped.</returns>
    public bool IsEquipped()
    {
        return this.equipped;
    }

    /// <summary>
    /// Sets the eqipped state. If the state is already active, may it be true or false,
    /// this call will be ignored.
    /// </summary>
    public void SetEquipped(bool equipped, GameObject player)
    {
        if (equipped)
        {
            if (!this.equipped)
            {
                // Activate equipped state.
                if (player == null)
                    Debug.LogError("Player to hold this item is null!");
                this.equipped = true;
                this.PlayerHolding = player;

                //Debug.Log("Item '" + Name + "' equipped to '" + player.name + "'");
            }
        }
        else
        {
            if (this.equipped)
            {
                // Remove equipped state.
                this.PlayerHolding = null;
                this.equipped = false;
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
            //Debug.Log("Loaded item '" + x.Prefab + "'");
        }

        Debug.Log("Loaded " + Items.Count + " items.");
    }

    public static string NameOf(string prefab)
    {
        Item i = GetItem(prefab);
        if (i == null)
            return null;

        return i.Name;
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
            NetworkLobbyManager.singleton.spawnPrefabs.Add(i.gameObject);
        }

        foreach (ThrowableInstance t in Resources.LoadAll<ThrowableInstance>("Items/"))
        {
            NetworkManager.singleton.spawnPrefabs.Add(t.gameObject);
        }
    }

    /// <summary>
    /// Creates a new instance of an object and spawns it into the world. Not networked.
    /// </summary>
    public static Item NewInstance(string prefab, Vector2 position, ItemDataX data)
    {
        // Create new instance of item.
        Item x = GetItem(prefab);
        Item newItem = Instantiate(x, position, Quaternion.identity);
        newItem.Data = data;

        return newItem;
    }

    /// <summary>
    /// Gets an item from the loaded items, this does not create a new instance.
    /// </summary>
    /// <param name="path">The prefab name of the item, as in Item.Prefab .</param>
    /// <returns>The item object, which is a prefab.</returns>
    public static Item GetItem(string prefab)
    {
        if (Items == null)
            LoadItems();
        if (string.IsNullOrEmpty(prefab))
        {
            return null;
        }
        if (!Items.ContainsKey(prefab))
        {
            Debug.LogError("Item not found! '" + prefab + "'");
            return null;
        }
        return Items[prefab];
    }

    public static bool ItemExists(string prefab)
    {
        if (Items == null)
            LoadItems();
        return Items.ContainsKey(prefab);
    }

    public static void Option_EquipGear(InventoryItemData x, string prefab)
    {
        PlayerInventory.Remove(x.Prefab, Vector2.zero, false); // Remove, do not drop.
        Player.Local.NetUtils.CmdSetGear(x.Item.GetComponent<GearItem>().Slot, prefab, x.Data == null ? null : x.Data.Serialize(), true);
    }

    public static void Option_Equip(InventoryItemData x, string prefab)
    {
        PlayerInventory.Remove(x.Prefab, Vector2.zero, false, 1); // Remove, do not drop.
        Player.Local.Holding.CmdEquip(x.Item.Prefab, Player.Local.gameObject, x.Data == null ? null : x.Data.Serialize());
    }

    public static void Option_Drop(InventoryItemData x, string prefab)
    {
        PlayerInventory.Remove(x.Prefab, Player.Local.transform.position, true);
    }

    public static void Option_Details(InventoryItemData x, string prefab)
    {
        PlayerInventory.inv.Inventory.DetailsView.Enter(x.Prefab);
    }

    public static void Option_QuickSlot(InventoryItemData x, string prefab)
    {
        PlayerInventory.inv.Inventory.QSI.Open = true;
        PlayerInventory.inv.Inventory.QSI.SelectedEvent.AddListener(UponQuickSlotSelect);
        tempSlotData = x;        
    }

    private static InventoryItemData tempSlotData;
    private static void UponQuickSlotSelect(int number)
    {
        if (tempSlotData.Data == null)
        {
            tempSlotData.Data = new ItemDataX();
            tempSlotData.Data.Add("Quick Slot", number);
        }
        else
        {
            tempSlotData.Data.Update("Quick Slot", number);
        }

        tempSlotData = null;
        PlayerInventory.inv.Inventory.Refresh = true;
    }

    public static void Option_ApplyAttachment(InventoryItemData x, string prefab)
    {
        // Set attachment...
        bool worked = Player.Local.Holding.Item.GetComponent<GunAttachments>().SetAttachment(x.Item.GetComponent<Attachment>().Type, x.Item);
        // Remove from inventory
        if(worked)
            PlayerInventory.Remove(x.Prefab, Vector2.zero, false, 1);
    }

    public static void Option_RemoveMagazine(InventoryItemData x, string prefab)
    {
        // Remove attachment...
        string old = x.Data.Get<string>("Magazine Attachment");
        x.Data.Remove("Magazine Attachment");

        // Give item back to player!
        PlayerInventory.Add(old, null, 1);
    }

    public static void Option_RemoveMuzzle(InventoryItemData x, string prefab)
    {
        // Remove attachment...
        string old = x.Data.Get<string>("Muzzle Attachment");
        x.Data.Remove("Muzzle Attachment");

        // Give item back to player!
        PlayerInventory.Add(old, null, 1);
    }

    public static void Option_RemoveSight(InventoryItemData x, string prefab)
    {
        // Remove attachment...
        string old = x.Data.Get<string>("Sight Attachment");
        x.Data.Remove("Sight Attachment");

        // Give item back to player!
        PlayerInventory.Add(old, null, 1);
    }

    public static void Option_RemoveUnderBarrel(InventoryItemData x, string prefab)
    {
        // Remove attachment...
        string old = x.Data.Get<string>("Under Barrel Attachment");
        x.Data.Remove("Under Barrel Attachment");

        // Give item back to player!
        PlayerInventory.Add(old, null, 1);
    }
}