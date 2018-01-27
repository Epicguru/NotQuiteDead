
using System.Collections.Generic;
using UnityEngine.Networking;

public class BuildingInventory : NetworkBehaviour
{
    // Attached to the player, records how many building items are carried at any given time.
    // Building items are Tiles and Furniture. They can be placed within the world.

    private Dictionary<string, BuildingItem> items = new Dictionary<string, BuildingItem>();

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
    }

    private void OnDestroy()
    {
        items.Clear();
        items = null;
    }
}