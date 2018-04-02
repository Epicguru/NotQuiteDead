using System;
using UnityEngine;

public static class WorldIO
{
    public static void SaveWorldState(World world)
    {
        // Get world state object.
        WorldSaveState sd = new WorldSaveState(world);
        string path = OutputUtils.RealitySaveDirectory + world.RealityName + OutputUtils.WorldStateSaveFile;

        OutputUtils.ObjectToFile(sd, path);
        Debug.Log("Saved world state to '" + path + "'");
    }

    public static void LoadWorldState(World world)
    {
        // Tryo to load from file.
        string path = OutputUtils.RealitySaveDirectory + world.RealityName + OutputUtils.WorldStateSaveFile;
        WorldSaveState sd = InputUtils.FileToObject<WorldSaveState>(path);

        if(sd != null)
        {
            sd.Apply(world);
            Debug.Log("Applied loaded world state...");
        }
        else
        {
            Debug.LogError("Null world state save file, no world state data applied.");
        }
    }

    public static WorldSaveState GetSaveState(string reality)
    {
        string path = OutputUtils.RealitySaveDirectory + reality + OutputUtils.WorldStateSaveFile;
        WorldSaveState sd = InputUtils.FileToObject<WorldSaveState>(path);

        return sd;
    }
}

public class WorldSaveState
{
    // Stuff like time, difficulty, etc.
    public float Time;
    public DateTime LastPlayed;
    public Vector2 SpawnPoint;
    public float SpawnRadius;

    public WorldSaveState()
    {

    }

    public WorldSaveState(World w)
    {
        Time = w.GameTime.GetTimeRaw();
        LastPlayed = DateTime.Now;
        SpawnPoint = w.SpawnPoint;
        SpawnRadius = w.SpawnRadius;
    }

    public void Apply(World world)
    {
        world.GameTime.SetTime(Time);
        world.SpawnPoint = SpawnPoint;
        world.SpawnRadius = SpawnRadius;
    }
}