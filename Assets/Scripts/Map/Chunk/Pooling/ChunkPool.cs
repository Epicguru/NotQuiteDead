using System.Collections.Generic;
using UnityEngine;

public class ChunkPool : MonoBehaviour
{
    public static ChunkPool Instance;
    public GameObject Prefab;

    public int Max = 100;

    [ReadOnly]
    public int Pooled;
    [ReadOnly]
    public int OnLease;
    [ReadOnly]
    public int Total;

    private List<Chunk> Spawned = new List<Chunk>();

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Spawned.Clear();
        Spawned = null;
        Instance = null;
    }

    public void DestroyPooled()
    {
        foreach (var chunk in Spawned)
        {
            Destroy(chunk);
        }
        Spawned.Clear();
    }

    private Chunk SpawnNewChunk()
    {
        return Instantiate(Prefab).GetComponent<Chunk>();
    }

    public Chunk GetChunk()
    {
        if(Spawned.Count > 0)
        {

        }
        else
        {

        }
    }

    public void ReturnChunk()
    {

    }
}