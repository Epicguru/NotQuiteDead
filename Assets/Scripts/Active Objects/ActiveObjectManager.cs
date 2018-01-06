using System.Collections.Generic;
using UnityEngine;

public class ActiveObjectManager : MonoBehaviour
{

    // Active object are objects that require chunks to be loaded around them.
    // This class manages chunk loading for those objects, and automatically unloads chunks that are no longer requested by anyone.

    public static ActiveObjectManager Instance { get; private set; }

    public float UnloadTime = 3f;
    public List<ActiveObject> Objects = new List<ActiveObject>();

    private Dictionary<int, float> RequestedChunks = new Dictionary<int, float>();
    private List<int> bin = new List<int>();

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public void Update()
    {
        CompileChunks();
        UpdateAndRemoveChunks();
        LoadChunks();

        DebugText.Log(Objects.Count + " Active Objects");
        DebugText.Log(RequestedChunks.Count + " Requested Chunks.");
    }

    public void CompileChunks()
    {
        foreach(ActiveObject o in Objects)
        {
            foreach(int requested in o.RequestedChunks)
            {
                if (!RequestedChunks.ContainsKey(requested))
                {
                    RequestedChunks.Add(requested, 0f);
                }
                else
                {
                    RequestedChunks[requested] = 0f;
                }
            }
        }
    }

    public void UpdateAndRemoveChunks()
    {
        bin.Clear();
        List<int> keys = new List<int>(RequestedChunks.Keys);
        foreach(int key in keys)
        {
            RequestedChunks[key] += Time.unscaledDeltaTime;
            if(RequestedChunks[key] >= UnloadTime)
            {
                // Remove me!
                bin.Add(key);
            }
        }

        foreach(int key in bin)
        {
            foreach (TileLayer layer in World.Instance.TileMap.GetAllLayers())
            {
                if(!layer.IsChunkUnloading(key))
                    layer.UnloadChunk(key);
            }
            RequestedChunks.Remove(key);
        }

        bin.Clear();
    }

    public void LoadChunks()
    {
        foreach(TileLayer layer in World.Instance.TileMap.GetAllLayers())
        {
            foreach (int index in RequestedChunks.Keys)
            {
                if(!layer.IsChunkLoaded(index) && !layer.IsChunkLoading(index))
                {
                    Vector2Int pos = layer.GetChunkCoordsFromIndex(index);
                    layer.LoadChunk(pos.x, pos.y);
                }
            }
        }
    }
}