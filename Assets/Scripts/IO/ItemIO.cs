﻿
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public static class ItemIO
{
    public static ItemSaveData ItemToSaveData(Item i)
    {
        if (i == null)
        {
            Debug.LogError("Null item to get save data from.");
            return null;
        }

        string prefab = i.Prefab;

        ItemSaveData sd = i.GetSaveData();
        if(sd == null)
        {
            Debug.LogError("Null save data returned by item '" + i.Prefab + "'");
            return null;
        }

        return sd;
    }

    public static Item[] FilterItems(params Item[] items)
    {
        return (from x in items where x.ShouldSerialize() select x).ToArray();
    }

    public static void ItemsToFile(string reality, params Item[] items)
    {
        items = FilterItems(items);
        ItemSaveData[] saveData = new ItemSaveData[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            ItemSaveData sd = ItemToSaveData(items[i]);
            saveData[i] = sd;
        }

        string filePath = OutputUtils.RealitySaveDirectory + reality + OutputUtils.ItemSaveFile;
        Debug.Log("Saving " + items.Length + " items to '" + filePath + "'");
        OutputUtils.ObjectToFile(saveData, filePath, new JsonSerializerSettings() { Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
    }

    public static ItemSaveData[] FileToSaveDatas(string reality)
    {
        if (string.IsNullOrEmpty(reality))
        {
            Debug.LogError("Reality is null or empty!");
            return null;
        }

        string filePath = OutputUtils.RealitySaveDirectory + reality + OutputUtils.ItemSaveFile;
        if (!File.Exists(filePath))
        {
            Debug.LogError("File '" + filePath + "' does not exist!");
            return null;
        }

        ItemSaveData[] data = InputUtils.FileToObject<ItemSaveData[]>(filePath);
        return data;
    }

    public static void SaveDataToWorldItems(ItemSaveData[] data)
    {
        // Should only be called on server!
        for (int i = 0; i < data.Length; i++)
        {
            ItemSaveData save = data[i];
            if (!Item.ItemExists(save.Prefab))
            {
                Debug.LogError("Item '" + save.Prefab + "' not found when loading items from item save data! (Item #" + i + ")");
                continue;
            }

            Item instance = Item.NewInstance(save.Prefab, new Vector2(save.X, save.Y), save.Data);
            NetworkServer.Spawn(instance.gameObject);
        }
    }

    public static void FileToWorldItems(string reality)
    {
        SaveDataToWorldItems(FileToSaveDatas(reality));
    }
}

public class ItemSaveData
{
    public string Prefab;
    public float X, Y;
    public ItemData Data;

    public ItemSaveData()
    {

    }

    public ItemSaveData(Item item)
    {
        Prefab = item.Prefab;
        Vector2 pos = item.transform.position;
        X = pos.x;
        Y = pos.y;
        item.RequestDataUpdate();
        Data = item.Data;
    }
}