using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TiledMap : MonoBehaviour
{
    public static TiledMap _Instance;
    public Tilemap Foreground;
    public Tilemap Background;
    public Dictionary<string, GameTile> CachedTiles = new Dictionary<string, GameTile>(); 

    public void Awake()
    {
        _Instance = this;
    }

    public Tilemap GetLayer(TileMapLayer layer)
    {
        switch (layer)
        {
            case TileMapLayer.BACKGROUND:
                return Background;
            case TileMapLayer.FOREGROUND:
                return Foreground;
        }

        return null;
    }

    public GameTile GetTile(string path)
    {
        if (CachedTiles.ContainsKey(path))
        {
            return CachedTiles[path];
        }
        else
        {
            GameTile tile = Resources.Load<GameTile>(path);

            if(tile == null)
            {
                Debug.LogError("Tile at path '" + path + "' not found!");
                return null;
            }
            else
            {
                CachedTiles.Add(path, tile);
                Debug.Log("Cached tile '" + path + "'");
                return tile;
            }
        }
    }

    public void Update()
    {
        Vector3Int pos = Foreground.WorldToCell(InputManager.GetMousePos());
        DebugText.Log("Mouse Tile Position: " + pos);
        TileBase t = Foreground.GetTile(pos);
        DebugText.Log("Tile Under Mouse: " + (t == null ? "null" : t.name));
        DebugText.Log("Foreground Bounds: " + Foreground.cellBounds);
    }
}

public enum TileMapLayer : byte
{
    BACKGROUND,
    FOREGROUND
}