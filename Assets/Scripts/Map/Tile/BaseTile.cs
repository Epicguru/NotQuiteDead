
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileName", menuName = "Tiles/Base Tile", order = 1)]
public class BaseTile : ScriptableObject
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
        if (!ContainsTile(prefab))
        {
            Debug.LogError("No tile prefab found for '" + prefab + "'");
            return null;
        }

        return tiles[prefab.Trim()];
    }

    public static bool ContainsTile(string prefab)
    {
        return tiles.ContainsKey(prefab.Trim());
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
            tiles.Add(tile.Prefab.Trim(), tile);
        }

        Debug.Log("Loaded " + tiles.Count + " tiles from resources.");
    }
}