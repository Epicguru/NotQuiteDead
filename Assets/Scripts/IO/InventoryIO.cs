
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryIO
{
    // IO for inventory and player.
    // EVERYTHING must be called from the SERVER ONLY!

    public static void SaveInventory(string reality)
    {
        if (string.IsNullOrEmpty(reality))
        {
            Debug.LogError("Reality is null or em empty, cannot save inventory items.");
            return;
        }

        // Make array of inventory item data.
        List<InventoryItemData> items = PlayerInventory.inv.Inventory.Contents;
        InventoryItemData[] array = items.ToArray();
        items = null;

        // Get save file path.
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.InventoryItemSaveFile;

        Debug.Log("Saving " + array.Length + " inventory items to '" + path + "'");

        // Save to file.
        OutputUtils.ObjectToFile(array, path);
    }

    public static void LoadInventory(string reality, bool clearInventory = true)
    {
        if (string.IsNullOrEmpty(reality))
        {
            Debug.LogError("Reality is null or em empty, cannot load inventory items.");
            return;
        }

        // Get file path.
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.InventoryItemSaveFile;

        // Make the list 
        InventoryItemData[] data = InputUtils.FileToObject<InventoryItemData[]>(path);

        if(data != null)
        {
            if (clearInventory)
            {
                PlayerInventory.inv.Inventory.Clear();
            }

            // Apply to the player.
            PlayerInventory.inv.Inventory.Contents = new List<InventoryItemData>(data);
        }
        else
        {
            Debug.LogError("Error loading inventory items from file.");
            return;
        }
    }

    public static void SaveGear(string reality, Player player)
    {
        if (string.IsNullOrEmpty(reality))
        {
            Debug.LogError("Reality is null or em empty, cannot save gear items.");
            return;
        }
        if(player == null)
        {
            Debug.LogError("Null player, cannot perform IO operation for gear items.");
            return;
        }

        // Get all the slots, and then get save data from each slot.
        List<GearSaveData> saveData = new List<GearSaveData>();
        Debug.Log("Saving " + (player.GearMap.Count - 1) + " gear slots...");
        foreach(var slotName in player.GearMap.Keys)
        {
            if(slotName == "Hands")
            {
                continue;
            }
            BodyGear bg = player.GearMap[slotName];
            GearSaveData sd = bg.GetSaveData();
            if(sd == null)
            {
                Debug.LogError("Gear slot '" + slotName + "' gave null save data!");
                continue;
            }

            saveData.Add(sd);
        }

        // Make array.
        GearSaveData[] array = saveData.ToArray();
        saveData.Clear();

        // Get file path.
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.GearItemSaveFile;

        // Save to file.
        OutputUtils.ObjectToFile(array, path);
    }

    public static void LoadGear(string reality, Player player)
    {
        if (string.IsNullOrEmpty(reality))
        {
            Debug.LogError("Reality is null or em empty, cannot load gear items.");
            return;
        }
        if (player == null)
        {
            Debug.LogError("Null player, cannot perform IO operation for gear items.");
            return;
        }

        // Get path.
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.GearItemSaveFile;

        // Make array.
        GearSaveData[] array = InputUtils.FileToObject<GearSaveData[]>(path);

        if(array == null)
        {
            Debug.LogError("Null gear file, removing all player gear...");
            foreach (var key in player.GearMap.Keys)
            {
                player.GearMap[key].SetItem(player.gameObject, null, null, false);
            }
            return;
        }

        foreach (var sd in array)
        {
            if(sd != null)
            {
                string slot = sd.Slot;
                if (player.GearMap.ContainsKey(slot))
                {
                    if (string.IsNullOrEmpty(sd.Prefab))
                    {
                        // Remove any gear in that slot without returning the previous item.
                        player.GearMap[slot].SetItem(player.gameObject, null, null, false);
                    }
                    else
                    {
                        Item i = Item.GetItem(sd.Prefab);
                        if(i == null)
                        {
                            Debug.LogError("Item load for gear slot '" + slot + "' could not be found (" + sd.Prefab + ")!");
                            player.GearMap[slot].SetItem(player.gameObject, null, null, false);
                        }
                        else
                        {
                            // Give loaded item to player, but do not return old one.
                            player.GearMap[slot].SetItem(player.gameObject, i, sd.Data, false);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Saved gear slot '" + slot + "' does not exist on the player '" + player.Name + "'!");
                    continue;
                }
            }
        }
    }

    public static void SaveHolding(string reality, Player player)
    {
        if (string.IsNullOrEmpty(reality))
        {
            Debug.LogError("Reality is null or em empty, cannot save held item.");
            return;
        }
        if (player == null)
        {
            Debug.LogError("Null player, cannot perform IO operation for held item.");
            return;
        }

        // Get file path.
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.HeldItemSaveFile;

        // Get the player holding monobehaviour.
        PlayerHolding holding = player.Holding;

        if(holding.Item == null)
        {
            OutputUtils.ObjectToFile(null, path);
        }
        else
        {
            ItemSaveData sd = holding.Item.GetSaveData();
            OutputUtils.ObjectToFile(sd, path);
        }
        // Done!
    }

    public static void LoadHolding(string reality, Player player)
    {
        if (string.IsNullOrEmpty(reality))
        {
            Debug.LogError("Reality is null or em empty, cannot load held item.");
            return;
        }
        if (player == null)
        {
            Debug.LogError("Null player, cannot perform IO operation for held item.");
            return;
        }

        // Get file path.
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.HeldItemSaveFile;

        // Get the player holding monobehaviour.
        PlayerHolding holding = player.Holding;

        ItemSaveData sd = InputUtils.FileToObject<ItemSaveData>(path);

        // Apply!
        if(sd == null || string.IsNullOrEmpty(sd.Prefab))
        {
            holding.CmdDrop(false, true, player.gameObject, null);
        }
        else
        {
            holding.ServerEquip(sd.Prefab, player.gameObject, sd.Data.Serialize());
        }
    }
}