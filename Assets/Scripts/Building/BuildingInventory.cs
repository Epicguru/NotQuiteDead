
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BuildingInventory : NetworkBehaviour
{
    // Attached to the player, records how many building items are carried at any given time.
    // Building items are Tiles and Furniture. They can be placed within the world.

    public int ActionID { get; private set; }

    private Dictionary<string, BuildingItem> items = new Dictionary<string, BuildingItem>();

    public bool ContainsItem(string prefab)
    {
        return items.ContainsKey(prefab);
    }

    public BuildingItem GetItem(string prefab)
    {
        if (!items.ContainsKey(prefab))
        {
            Debug.LogError("Item could not be found for that prefab name, it might not be in the inventory right now. (" + prefab + ")");
            return null;
        }

        return items[prefab];
    }

    public void RemoveItems(string prefab, int count)
    {
        if (count < 0)
            count *= -1;

        if (items.ContainsKey(prefab))
        {
            BuildingItem i = items[prefab];
            i.Count -= count;
            if (i.Count <= 0)
                items.Remove(prefab);
        }
    }

    public void AddItems(BaseTile tile, int count)
    {
        if (count <= 0)
            return;
        if (tile == null)
            return;

        BuildingItem item = new BuildingItem(tile, count);
        AddItem(item);
    }

    public void AddItems(Furniture furn, int count)
    {
        if (count <= 0)
            return;
        if (furn == null)
            return;

        BuildingItem item = new BuildingItem(furn, count);
        AddItem(item);
    }

    private void AddItem(BuildingItem item)
    {
        // Assumes that the item.Count value has already been assigned.
        // We might already have this item in our inventory.
        if (items.ContainsKey(item.Prefab))
        {
            items[item.Prefab].Count += item.Count;
        }
        else
        {
            // We don't already have it, so add it!
            items.Add(item.Prefab, item);
        }

        ActionID++;
    }

    public Dictionary<string, BuildingItem>.ValueCollection GetItems()
    {
        return this.items.Values;
    }

    public void SetItems(BuildingItem[] items)
    {
        this.items = new Dictionary<string, BuildingItem>();
        foreach (var item in items)
        {
            if(item != null)
                this.items.Add(item.Prefab, item);
        }
    }

    private void OnDestroy()
    {
        items.Clear();
        items = null;
    }
}