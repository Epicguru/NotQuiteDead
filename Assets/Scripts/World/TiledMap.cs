using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class TiledMap : NetworkBehaviour
{
    [Range(5, 200)]
    public int Width = 200;
    [Range(5, 200)]
    public int Height = 200;

    [Range(0f, 1f)]
    public float IslandMin = 0.1f;
    [Range(0f, 1f)]
    public float DirtMin = 0.2f;
    [Range(0f, 1f)]
    public float StoneMin = 0.3f;

    public Texture2D IslandMask;

    public Tile[] Prefabs;

    private Tile[,] tiles;

    public void Start()
    {
        // Create tiles.

        Gen();
    }

    public void Gen()
    {
        tiles = new Tile[Width, Height];

        Debug.Log("Creating " + Width + "x" + Height + " tiles. Using " + Prefabs.Length + " prefabs.");

        NoiseMap noise = new NoiseMap(Width, Height);
        NoiseMap noise2 = new NoiseMap(Width, Height);
        noise.Multiply(noise2);// TEST
        noise.Multiply(IslandMask); // Make island shape.

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                // Fills with first tile.
                GameObject prefab = GetPrefab(noise.Sample(x, y));
                tiles[x, y] = Instantiate(prefab, transform).GetComponent<Tile>();
                tiles[x, y].SetPosition(this, x, y);
            }
        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                // Update physics
                Ping(x, y, true); // Solo ping
            }
        }
    }

    private GameObject GetPrefab(float height)
    {
        // TODO make a better system.
        if (height <= IslandMin)
            return Prefabs[0].gameObject;
        if (height > IslandMin && height <= DirtMin)
            return Prefabs[1].gameObject;
        if (height > DirtMin && height <= StoneMin)
            return Prefabs[2].gameObject;
        if (height > StoneMin && height <= 1f)
            return Prefabs[3].gameObject;

        return null;
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public Tile GetTile(int x, int y)
    {
        if (!InBounds(x, y))
            return null;

        return tiles[x, y];
    }

    public void SetTile(int x, int y, Tile tile, bool ping = true, bool soloPing = false)
    {
        if (tiles[x, y] != null)
            Destroy(tiles[x, y].gameObject);
        tiles[x, y] = null;

        if (tile == null)
            return;

        GameObject prefab = tile.gameObject;
        tiles[x, y] = Instantiate(prefab, transform).GetComponent<Tile>();
        tiles[x, y].SetPosition(this, x, y);

        if (ping)
        {
            Ping(x, y, soloPing);
        }
    }

    public void Ping(int x, int y, bool solo = false)
    {
        // Updates physics for the tile and all surrounding tiles.
        // If solo, just the one tile.

        Tile t = GetTile(x, y);
        if (t == null)
            return;

        t.Ping();

        if (solo)
            return;

        Tile z = t.GetNeighbour(-1, 0);
        if (z != null) z.Ping();
        z = t.GetNeighbour(1, 0);
        if (z != null) z.Ping();
        z = t.GetNeighbour(0, 1);
        if (z != null) z.Ping();
        z = t.GetNeighbour(0, -1);
        if (z != null) z.Ping();
    }

    public void Dispose()
    {
        if (tiles == null)
            return;

        foreach(Tile t in tiles)
        {
            Destroy(t.gameObject);
        }
        tiles = null;
    }
}
