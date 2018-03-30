using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryUI : MonoBehaviour
{
    public Inventory Inventory
    {
        get
        {
            return inventory;
        }
        set
        {
            if (value == inventory)
                return;

            inventory = value;
            InventoryChanged(inventory);
        }
    }
    [Header("Data")]
    [SerializeField]
    private Inventory inventory;

    [Header("References")]
    public RectTransform ContentParent;
    public InventoryItemUI Prefab;

    public ItemClickedEvent OnItemClicked = new ItemClickedEvent();

    private List<InventoryItemUI> spawned = new List<InventoryItemUI>();

    private void InventoryChanged(Inventory newInventory)
    {
        DestroySpawned();

        if(newInventory != null)
        {
            SpawnItems(newInventory);
        }
    }

    public void SpawnItems(Inventory inv)
    {
        if (inv == null)
            return;

        foreach (var pair in inv.Content)
        {
            Item itemPrefab = Item.GetItem(pair.Key);

            if (itemPrefab == null)
            {
                Debug.LogError("Cannot spawn '{0}' as an inventory UI item! Item prefab not found!".Form(pair.Key));
                continue;
            }

            // Spawn based on the stackablilty of the item.
            if (itemPrefab.CanStack)
            {
                ItemStack stack = pair.Value[0];

                // Spawn the item stack...
                var invItem = Instantiate(Prefab, ContentParent);
                invItem.Prefab = stack.Prefab;
                invItem.Count = stack.Count;
                invItem.Name = itemPrefab.Name;

                spawned.Add(invItem);
            }
            else
            {
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    ItemStack stack = pair.Value[i];

                    // Spawn the item...
                    var invItem = Instantiate(Prefab, ContentParent);
                    invItem.Prefab = stack.Prefab;
                    invItem.Name = itemPrefab.Name;
                    invItem.Count = 1; // Always one.

                    spawned.Add(invItem);
                }
            }
        }
    }

    public void DestroySpawned()
    {
        foreach (var go in spawned)
        {
            if(go != null)
            {
                Destroy(go);
            }
        }
        spawned.Clear();
    }

    public void OnOpen()
    {
        InventoryChanged(Inventory);
    }

    public void OnClose()
    {
        DestroySpawned();
    }

    public void ItemClicked(ItemStack stack)
    {
        OnItemClicked.Invoke(stack);
        Debug.Log("Got click!");
    }
}

public class ItemClickedEvent : UnityEvent<ItemStack>
{

}