
using System.Collections.Generic;
using UnityEngine;

public static class FurnitureIO
{
    public static void SaveFurniture(string reality, params Furniture[] f)
    {
        if (string.IsNullOrWhiteSpace(reality))
        {
            Debug.LogError("Reality name is null or empty! Cannot save furniture!");
            return;
        }

        // Get file path...
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.FurnitureSaveDirectory + OutputUtils.FurnitureSaveFile;

        // Make save datas...
        var saves = GetSaveData(f);

        // Save to file!
        OutputUtils.ObjectToFile(saves, path);

        Debug.Log(string.Format("Saved {0}/{1} placed furniture objects to '{2}'", saves.Length, f.Length, path));
    }

    public static void LoadFurniture(World world)
    {
        // As always, only on server!
        if (world == null || string.IsNullOrWhiteSpace(world.RealityName))
        {
            Debug.LogError("World object or reality name is null! Cannot load furniture!");
            return;
        }

        // Get file path...
        string path = OutputUtils.RealitySaveDirectory + world.RealityName + OutputUtils.FurnitureSaveDirectory + OutputUtils.FurnitureSaveFile;

        // Make save datas from the file...
        FurnitureSaveData[] sds = InputUtils.FileToObject<FurnitureSaveData[]>(path);

        // Destroy all placed furniture...
        Furniture[] placed = GameObject.FindObjectsOfType<Furniture>();
        foreach (var f in placed)
        {
            GameObject.Destroy(f.gameObject);
        }
        Debug.Log("Destroyed " + placed.Length + " placed furnitures before load...");
        placed = null;

        if(sds == null)
        {
            Debug.LogError("No furniture got from the save file! Not placing any furniture.");
            return;
        }

        // Place the save datas within the world...
        foreach (var sd in sds)
        {
            if(sd == null)
            {
                continue;
            }

            sd.PlaceInWorld(world.Furniture);
        }
    }

    public static FurnitureSaveData[] GetSaveData(params Furniture[] furns)
    {
        List<FurnitureSaveData> sds = new List<FurnitureSaveData>();

        foreach (var f in furns)
        {
            if(f != null)
            {
                FurnitureSaveData sd = f.GetSaveData();
                if (sd == null)
                {
                    Debug.LogError(string.Format("Save data from furniture '{0}', class type '{1}', gave a null save data in the GetSaveData method. Resorting to applying a default save data. This may result in a loss of data.", f.Prefab, f.GetType().ToString()));
                    sd = new FurnitureSaveData(f);
                }

                sds.Add(sd);
            }
        }

        return sds.ToArray();
    }
}

/// <summary>
/// By default saves only furniture prefab and position.
/// Anything else requires this class to be inherited from and used in the GetSaveData method in Furniture.cs
/// </summary>
public class FurnitureSaveData
{
    public string Prefab;
    public int X, Y;

    public FurnitureSaveData()
    {

    }

    public FurnitureSaveData(Furniture f)
    {
        GetData(f);
    }

    public virtual void GetData(Furniture f)
    {
        if (f == null)
            return;

        X = f.X;
        Y = f.Y;
        Prefab = f.Prefab;
    }

    public virtual void PlaceInWorld(FurnitureManager fm)
    {
        if (fm == null)
            return;

        bool worked = fm.PlaceFurniture(Prefab, X, Y);

        if (!worked)
        {
            Debug.LogError("Failed to place '" + Prefab + "'! Data may be lost!");
        }
        else
        {
            Furniture f = fm.GetFurnitureAt(X, Y);
            if(f != null)
            {
                ApplyData(f);
            }
            else
            {
                Debug.LogError("Placed furniture '" + Prefab + "' at " + X + ", " + Y + " but requesting the placed object failed! Might result in data loss for placed furniture.");
            }
        }
    }

    public virtual void ApplyData(Furniture f)
    {
        if (f == null)
            return;

        f.SetPosition(X, Y);
        f.Prefab = Prefab; // Should already be the case...
    }
}