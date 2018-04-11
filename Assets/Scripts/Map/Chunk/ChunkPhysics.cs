using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPhysics : MonoBehaviour {

    public Chunk Chunk;
    public CompositeCollider2D Composite;
    public bool Enabled = true;

    [ReadOnly]
    public bool Dirty;
    int framesDirty = 0;

    public Collider2D[][] Colliders;

    public void LateUpdate()
    {
        if (Dirty)
        {
            framesDirty++;
            if(framesDirty == 2)
            {
                Dirty = false;
                framesDirty = 0;
                Composite.GenerateGeometry();
            }
        }
    }

    public void BuildMap()
    {
        RemoveAll();
        Colliders = new Collider2D[Chunk.Width][];
        for (int x = 0; x < Chunk.Width; x++)
        {
            Colliders[x] = new Collider2D[Chunk.Height];
        }
    }

    public void RemoveAll()
    {
        if (Colliders == null)
            return;
        for (int x = 0; x < Chunk.Width; x++)
        {
            for (int y = 0; y < Chunk.Height; y++)
            {
                RemoveCollider(x, y);
            }
        }

        Dirty = true;
    }

    public void AssignAll()
    {
        TileLayer layer = Chunk.Layer;
        for (int x = 0; x < Chunk.Width; x++)
        {
            for (int y = 0; y < Chunk.Height; y++)
            {
                int X = Chunk.X * Chunk.Width + x;
                int Y = Chunk.Y * Chunk.Height + y;

                BaseTile tile = layer.GetTile(X, Y);
                if(tile != null)
                {
                    if (tile.HasCollider)
                    {
                        AssignCollider(x, y);
                    }
                }
            }
        }

        Dirty = true;
    }

    public void RebuildAll()
    {
        RemoveAll();
        AssignAll();
    }

    public void AssignCollider(int x, int y)
    {
        if (!Enabled)
            return;


        if(!Chunk.InBounds(x, y))
        {
            Debug.LogError("Tile outside of chunk bounds! Cannot set physics body.");
            return;
        }

        if(Colliders[x][y] != null)
        {
            Destroy(Colliders[x][y].gameObject);
        }

        Dirty = true;

        var instance = ObjectPool.Instantiate(Spawnables.I.TileCollider, PoolType.TILE_COLLIDER, transform);
        Collider2D colliderInstance = instance.GetComponent<Collider2D>();

        instance.transform.localPosition = new Vector3((x * Chunk.Mesh.TileSize) + (Chunk.Mesh.TileSize * 0.5f), (y * Chunk.Mesh.TileSize) + (Chunk.Mesh.TileSize * 0.5f), 0);

        instance.name = instance.name.Replace("(Clone)", "");
        instance.name = instance.name + " (" + x + "," + y + ")";

        Colliders[x][y] = colliderInstance;

        Dirty = true;
    }

    public void RemoveCollider(int x, int y)
    {
        if (!Enabled)
            return;

        if (!ColliderExists(x, y))
            return;

        Collider2D c = Colliders[x][y];
        Colliders[x][y] = null;

        ObjectPool.Destroy(c.gameObject, PoolType.TILE_COLLIDER);

        Dirty = true;
    }

    public bool ColliderExists(int x, int y)
    {
        return Colliders[x][y] != null;
    }
}