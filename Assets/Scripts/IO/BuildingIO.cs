
using System.Linq;
using UnityEngine;

public static class BuildingIO
{
    public static void SaveBuildingInventory(string reality, Player player)
    {
        if(string.IsNullOrWhiteSpace(reality) || player == null)
        {
            Debug.LogError("Reality is null or player is null! Both must not be null!");
            return;
        }

        // Get all the items in the building inventory...
        var items = player.BuildingInventory.GetItems().ToArray();

        if(items == null)
        {
            Debug.LogError("Inventory building items for player '" + player.Name + "' are null!");
            return;
        }

        // Get path...
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.PlayerSaveDirectory + player.Name + OutputUtils.BuildingInventorySaveFile;

        // Save to file...
        OutputUtils.ObjectToFile(items, path);

        // Done!
        Debug.Log("Saved " + items.Length + " inventory building items to '" + path + "'");
    }

    public static void LoadBuildingInventory(string reality, Player player)
    {
        if (string.IsNullOrWhiteSpace(reality) || player == null)
        {
            Debug.LogError("Reality is null or player is null! Both must not be null!");
            return;
        }

        // Get path...
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.PlayerSaveDirectory + player.Name + OutputUtils.BuildingInventorySaveFile;

        // Load items from file.
        BuildingItem[] items = InputUtils.FileToObject<BuildingItem[]>(path);

        if(items == null)
        {
            Debug.LogError("Failed to load any inventory building items!");
            return;
        }

        // Apply the items...
        player.BuildingInventory.SetItems(items);
    }
}