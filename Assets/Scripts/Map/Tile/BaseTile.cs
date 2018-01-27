
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
    public string Name;

    [Tooltip("How valuable is this tile?")]
    public ItemRarity Rarity = ItemRarity.COMMON;

    [Tooltip("Can this tile be walked though.")]
    public bool Walkable = false;

    [Tooltip("A reference to the physics body.")]
    public Collider2D Physics;

    [Tooltip("The icon used in UI.")]
    public Sprite Icon;

    [Tooltip("The Render Data that is used to get sprites from.")]
    public TileRenderData RenderData;

    public static BaseTile GetTile(string prefab)
    {
        if (!ContainsTile(prefab))
        {
            Debug.LogError("No tile prefab found for '" + (prefab == null ? "<null>" : prefab) + "'");
            return null;
        }

        return tiles[prefab.Trim()];
    }

    public static Dictionary<string, BaseTile>.ValueCollection GetAllTiles()
    {
        return tiles.Values;
    }

    public static bool ContainsTile(string prefab)
    {
        return tiles.ContainsKey(prefab.Trim());
    }

    public static Dictionary<string, BaseTile> GetTileDictionary()
    {
        return tiles;
    }

    public virtual Sprite GetSprite(TileLayer layer, int x, int y)
    {
        return GetSprite(GetAutoIndex(layer, x, y));
    }

    public int GetAutoIndex(TileLayer layer, int x, int y)
    {
        return GetSpriteIndex(GetTileValue(layer, x, y, -1, 0), GetTileValue(layer, x, y, 0, 1), GetTileValue(layer, x, y, 1, 0), GetTileValue(layer, x, y, 0, -1));
    }

    public virtual Sprite GetSprite(int index)
    {
        return this.RenderData == null ? null : this.RenderData.GetSprite(index);
    }

    public virtual Sprite GetSprite(int left, int top, int right, int bottom)
    {
        return this.GetSprite(GetSpriteIndex(left, top, right, bottom));
    }

    public virtual int GetSpriteIndex(int left, int top, int right, int bottom)
    {
        return (left * 8) + (top * 4) + (right * 2) + bottom;
    }

    public virtual int GetTileValue(TileLayer layer, int x, int y, int offsetX, int offsetY)
    {
        if (layer == null)
            return 0;

        return layer.GetTile(x + offsetX, y + offsetY) == null ? 0 : 1;
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