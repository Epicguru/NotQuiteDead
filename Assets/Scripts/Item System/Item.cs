using System.Collections;
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

    public ItemOption[] CreateOptions()
    {
        ItemOption[] Options = new ItemOption[]
        {
            new ItemOption() { OptionName = "Drop", OnSelected = Option_Drop},
            new ItemOption() { OptionName = "Equip", OnSelected = Option_Equip},
            new ItemOption() { OptionName = "Details", OnSelected = Option_Details}
        };

        return Options;
    }

    public void Update()
    {
        // Does not matter where this is running...
        netTransform.enabled = (transform.parent == null);

        // Layer
        SetLayer(IsEquipped() ? "Equipped Items" : "Dropped Items");

        UpdateParent();
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

                Debug.Log("Item '" + Name + "' equipped to '" + player.name + "'");
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
        return Items[prefab];
    }

    public static void Option_Equip(InventoryItem x)
    {
        x.Inventory.RemoveItem(x, Vector2.zero, false); // Remove, do not drop.
        Player.Local.Holding.CmdEquip(x.Item.Prefab, Player.Local.gameObject);
    }

    public static void Option_Drop(InventoryItem x)
    {
        x.Inventory.RemoveItem(x, Player.Local.transform.position, true);
    }

    public static void Option_Details(InventoryItem x)
    {
        x.Inventory.DetailsView.Enter(x);
    }
}