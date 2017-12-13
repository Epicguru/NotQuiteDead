
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTile : TileBackend
{
    // Static stuff.
    private static Dictionary<string, BaseTile> tiles = new Dictionary<string, BaseTile>();

    [Tooltip("The constant prefab name. Can be anything, as long as it never changes and is unique.")]
    public string Prefab;

    [Tooltip("The visual name of the tile.")]
    public string Name; // TODO add ID or Prefab

    [Tooltip("Is this considered solid? Can AI walk through it?")]
    public bool Solid = false;

    [Tooltip("A reference to the physics body.")]
    public Collider2D Physics;

    [Tooltip("TODO Add different textures.")]
    public Sprite Texture;

    private TileLayer layer;

    public static BaseTile GetTile(string prefab)
    {
        if (!tiles.ContainsKey(prefab))
        {
            Debug.LogError("No tile prefab found for '" + prefab + "'");
            return null;
        }

        return tiles[prefab];
    }

    public static Dictionary<string, BaseTile> GetTileDictionary()
    {
        return tiles;
    }

    public static void LoadPrefabs()
    {
        tiles.Clear();

        BaseTile[] t =  Resources.LoadAll<BaseTile>("Tiles");

        foreach(BaseTile tile in t)
        {
            tiles.Add(tile.Prefab, tile);
        }

        Debug.Log("Loaded " + tiles.Count + " tiles from resources.");
    }

    public bool SameTypeAs(BaseTile other)
    {
        return other == null ? false : other.Prefab == this.Prefab;
    }

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