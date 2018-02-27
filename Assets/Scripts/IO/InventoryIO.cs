
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryIO
{
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
        items.Clear();
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
            Debug.LogError("Reality is null or em empty, cannot save inventory items.");
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

    public static void SaveGear()
    {

    }

    public static void LoadGear()
    {

    }
}