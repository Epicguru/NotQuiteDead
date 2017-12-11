
using UnityEngine;

public abstract class BaseTile : TileBackend
{
    [Tooltip("The constant prefab name. Can be anything, as long as it never changes and is unique.")]
    public string Prefab;

    [Tooltip("The visual name of the tile.")]
    public string Name; // TODO add ID or Prefab

    [Tooltip("Is this considered solid? Can AI walk through it?")]
    public bool Solid = false;

    [Tooltip("A reference to the physics body.")]
    public Collider2D Physics;

    private TileLayer layer;

    public Chunk GetChunk()
    {
        return this.layer.GetChunkFromChunkCoords(GetChunkX(), GetChunkY());
    }

    public int GetChunkIndex()
    {
        return this.layer.GetChunkIndex(GetChunkX(), GetChunkY());
    }

    public int GetChunkX()
    {
        return this.GetX() / layer.ChunkSize;
    }

    public int GetChunkY()
    {
        return this.GetY() / layer.ChunkSize;
    }

    public TileLayer GetLayer()
    {
        return this.layer;
    }
}