using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(ItemPickup), typeof(NetworkTransform))]
public class Item : NetworkBehaviour
{

    /*
    * Represents an item that can be found, crafted, dropped etc.
    * This is attached to the actual game object that is the item. Items can be stored in inventories.
    * Some items are equipable, but the equping actions will be handled in another script.
    */

    [Tooltip("The location of the prefab for this item, in the resouces folder.")]
    public string Prefab = "Resources/Items/";

    [Tooltip("The internal (TODO) name of this item.")]
    public string Name = "Default Item Name";

    [Tooltip("The rarity (tier) of the item.")]
    public ItemRarity Rarity;

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
    private bool dropped = false;
    [SyncVar]
    private bool equiped = false;
    private NetworkTransform netTransform;
    [SyncVar]
    private GameObject PlayerHolding;

    public void Start()
    {
        netTransform = GetComponent<NetworkTransform>();
    }

    public void Update()
    {
        // Does not matter where this is running...
        netTransform.enabled = (transform.parent == null);
        UpdateParent();
    }

    public void UpdateParent()
    {
        if (IsEquipped())
        {
            transform.SetParent(PlayerHolding.transform);
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
    /// Gets the Dropped state. You can handle dropped items and the concequences of that in any script
    /// that you like.
    /// </summary>
    /// <returns>True if this item is loose on the ground. False if it is stored in an inventory or equipped.</returns>
    public bool IsDropped()
    {
        return dropped;
    }

    /// <summary>
    /// Sets the dropped state. If the state is already active, may it be true or false,
    /// this call will be ignored.
    /// </summary>
    public void SetDropped(bool dropped)
    {
        if (dropped)
        {
            if (!this.dropped)
            {
                // Activate equipped state.

                this.dropped = true;
            }
        }
        else
        {
            if (this.dropped)
            {
                // Remove equipped state.

                this.dropped = false;
            }
        }
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

    /// <summary>
    /// Creates a new instance of an object and spawns it into the world.
    /// </summary>
    public static Item NewInstance(string path)
    {
        // Create new instance of item.
        Item prefab = FindItem(path);
        // TODO DATA!
        Item newItem = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        return newItem;
    }

    /// <summary>
    /// Gets an item from the resouces folder, this does not create a new instance.
    /// </summary>
    /// <param name="path">The path to the item, as in Item.Prefab .</param>
    /// <returns>The item object, which is a prefab.</returns>
    public static Item FindItem(string path)
    {
        return Resources.Load<GameObject>(path).GetComponent<Item>();
    }
}