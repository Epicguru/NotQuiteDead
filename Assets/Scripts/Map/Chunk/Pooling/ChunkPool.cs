using System.Collections.Generic;
using UnityEngine;

public class ChunkPool : MonoBehaviour
{
    public static ChunkPool Instance;
    public GameObject Prefab;

    [ReadOnly]
    public int Pooled;
    [ReadOnly]
    public int OnLease;
    [ReadOnly]
    public int Total;

    private List<Chunk> Pool = new List<Chunk>();

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Pool.Clear();
        Pool = null;
        Instance = null;
    }

    public void DestroyPooled()
    {
        foreach (var chunk in Pool)
        {
            Destroy(chunk);
        }
        Pool.Clear();
    }

    private Chunk SpawnNewChunk()
    {
        Total++;
        return Instantiate(Prefab).GetComponent<Chunk>();
    }

    public Chunk GetChunk(Vector2 position, Transform parent = null)
    {
        if(Pool.Count > 0)
        {
            Pooled--;
            OnLease++;
            var chunk = Pool[0];
            Pool.RemoveAt(0);

            chunk.gameObject.SetActive(true);
            chunk.transform.SetParent(parent);
            chunk.transform.position = position;

            return chunk;
        }
        else
        {
            var chunk = SpawnNewChunk();
            OnLease++;

            return chunk;
        }
    }

    public void ReturnChunk(Chunk c)
    {
        if (c == null)
            return;

        OnLease--;
        Pooled++;

        Pool.Add(c);
    }
}