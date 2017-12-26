using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class TileLayer : NetworkBehaviour
{
    public string Name;

    public Chunk ChunkPrefab;
    public int ChunkSize = 16;

    public int Width { get; private set; }
    public int Height { get; private set; }
    public int WidthInChunks { get; private set; }
    public int HeightInChunks { get; private set; }

    // Map of chunks, where key is the index. (The calculated index, x * height + y).
    public Dictionary<int, Chunk> Chunks = new Dictionary<int, Chunk>();
    private List<int> unloading = new List<int>();
    private List<int> loading = new List<int>();
    private BaseTile[][] Tiles;

    public void Update()
    {
        int x = (int)InputManager.GetMousePos().x;
        int y = (int)InputManager.GetMousePos().y;

        if(InLayerBounds(x, y))
        {
            if (InputManager.InputPressed("Shoot"))
            {
                SetTile(BaseTile.GetTile("Dirt"), x, y);
            }
        }
    }

    public void Create(int width, int height)
    {
        Debug.Log("Creating map layer '" + name + "', size in tiles: " + width + "x" + height);

        if (width <= 0 || height <= 0)
        {
            Debug.LogError("Width and height must be one or more!");
            return;
        }

        this.Width = width;
        this.Height = height;

        CreateTileArray();

        this.WidthInChunks = CalculateChunksFromTiles(Width, ChunkSize);
        this.HeightInChunks = CalculateChunksFromTiles(Height, ChunkSize);

        Debug.Log("Using a chunk size of " + ChunkSize + ", there are " + WidthInChunks + " horizontal chunks and " + HeightInChunks + " vertical chunks.");
        Debug.Log("Real size " + ChunkSize * WidthInChunks * HeightInChunks + " tiles, " + ChunkSize * WidthInChunks + "x" + ChunkSize * HeightInChunks + ".");
    }

    private void CreateTileArray()
    {
        this.Tiles = new BaseTile[Width][];
        for (int i = 0; i < Width; i++)
        {
            Tiles[i] = new BaseTile[Height];
        }
    }

    public BaseTile GetTile(int x, int y)
    {
        if (!InLayerBounds(x, y))
            return null;

        if (!IsChunkLoaded(GetChunkIndexFromTileCoords(x, y)))
            return null;

        return Tiles[x][y];
    }

    /// <summary>
    /// Places a tile, can be called from the server only. Places a tile on the server and on all clients.
    /// </summary>
    /// <param name="tile">The tile PREFAB.</param>
    /// <param name="x">The x position, in tiles.</param>
    /// <param name="y">The y position, in tiles.</param>
    /// <returns>True if the operation was successful.</returns>
    [Server]
    public bool SetTile(BaseTile tile, int x, int y)
    {
        if(!CanPlaceTile(x, y))
        {
            Debug.LogError("Cannot place tile at " + x + ", " + y + " in layer '" + Name + "'.");
            return false;
        }

        // Networking here!
        // TODO

        // For now, just place tile in position.
        BaseTile oldTile = Tiles[x][y];

        if(oldTile == tile)
        {
            // No need to change anything, already set that tile there!
            return false;
        }

        if(oldTile != null)
        {
            TileRemoved(x, y, oldTile);
        }

        Tiles[x][y] = tile;

        TilePlaced(x, y, tile);

        return true;
    }

    [Server]
    public void SetTiles(BaseTile[][] tiles, int x, int y)
    {
        if (tiles == null)
            return;

        for (int X = x; X < x + tiles.Length; X++)
        {
            BaseTile[] yArray = tiles[X - x];

            for (int Y = y; Y < y + yArray.Length; Y++)
            {
                if(CanPlaceTile(X, Y))
                    SetTile(yArray[Y - y], X, Y);
            }
        }
    }

    public void TilePlaced(int x, int y, BaseTile newTile)
    {
        if (newTile == null)
        {
            Debug.LogError("Cannot place null tile! FOR NOW TODO ADDME!");
            return;
        }

        int chunkIndex = GetChunkIndexFromTileCoords(x, y);

        if (!IsChunkLoaded(chunkIndex))
        {
            Debug.LogError("TilePlaced called in unloaded chunk!");
            return;
        }

        Chunk chunk = GetChunkFromIndex(chunkIndex);

        int localX = x - (ChunkSize * chunk.X);
        int localY = y - (ChunkSize * chunk.Y);

        if (newTile.Physics)
        {
            chunk.Physics.AssignCollider(newTile.Physics, localX, localY);
        }

        if (newTile.Texture)
        {
            chunk.Texture.SetTile(newTile.Texture, localX, localY);
        }
    }

    public void TileRemoved(int x, int y, BaseTile oldTile)
    {

    }

    public bool CanPlaceTile(int x, int y)
    {
        if(!InChunkBounds(x, y))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Is the position within layer bounds AND within a LOADED chunk?
    /// </summary>
    /// <param name="x">Tile X position.</param>
    /// <param name="y">Tile Y position.</param>
    /// <returns>True if within layer bounds and also within a currently loaded chunk.</returns>
    public bool InChunkBounds(int x, int y)
    {
        if (!InLayerBounds(x, y))
        {
            return false;
        }

        if(IsChunkLoaded(GetChunkIndexFromTileCoords(x, y)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool InLayerBounds(int x, int y)
    {
        if (x < 0 || x >= Width)
            return false;
        if (y < 0 || y >= Height)
            return false;

        return true;
    }

    public void LoadChunk(int x, int y)
    {
        // TODO, fixme.
        // TODO, pool chunks.
        // This would load from file.

        if (!IsChunkInBounds(x, y))
        {
            Debug.LogError("Chunk coordinate " + x + ", " + y + " (chunk space) is not in bounds.");
            return;
        }

        if (IsChunkLoaded(x, y))
        {
            Debug.LogError("Chunk at " + x + ", " + y + " (chunk space) is already loaded, cannot load again!");
            return;
        }

        int index = GetChunkIndex(x, y);

        if (IsChunkLoading(index))
        {
            Debug.LogError("Chunk at " + x + ", " + y + " is already loading.");
            return;
        }

        // Flag as loading...
        loading.Add(index);

        // Instantiate object, TODO pool me.
        Chunk newChunk = Instantiate(ChunkPrefab.gameObject, transform).GetComponent<Chunk>();

        // Create the chunk object.
        newChunk.Create(x, y, ChunkSize, ChunkSize, index);

        // Add to the chunks list.
        Chunks.Add(index, newChunk);

        // Check if is saved data...
        if(ChunkIO.IsChunkSaved("James' Reality", Name, x, y))
        {
            // Load from disk.
            ChunkIO.LoadChunk("James' Reality", Name, x, y, ChunkSize, ChunkLoaded, ChunkLoadError);
        }
        else
        {
            // TODO add something, like generating this chunk.
            // Just leave a blank chunk, lol.
            loading.Remove(index);
        }
    }

    private void ChunkLoaded(object[] args)
    {
        // Get values.
        bool worked = (bool)args[0];
        int chunkX = (int)args[1];
        int chunkY = (int)args[2];

        int index = GetChunkIndex(chunkX, chunkY);

        if (!worked)
        {
            loading.Remove(index);
        }

        BaseTile[][] tiles = (BaseTile[][])args[3];

        // TODO LEFT HERE
        SetTiles(tiles, chunkX * ChunkSize, chunkY * ChunkSize);

        loading.Remove(index);
    }

    private void ChunkLoadError(string error)
    {
        Debug.LogError("[CHUNK LOAD] " + error);
    }

    public void UnloadChunk(int x, int y)
    {
        // TODO, fixme.
        // TODO, pool chunks.

        // This would save to file.

        if (!IsChunkInBounds(x, y))
        {
            Debug.LogError("Chunk coordinate " + x + ", " + y + " (chunk space) is not in bounds.");
            return;
        }

        if (!IsChunkLoaded(x, y))
        {
            Debug.LogError("Chunk at " + x + ", " + y + " (chunk space) is not loaded, cannot unload!");
            return;
        }

        UnloadChunk(GetChunkIndex(x, y));        
    }

    public void UnloadChunk(int index)
    {
        if (!IsChunkLoaded(index))
        {
            Debug.LogError("The chunk for index " + index + " is not loaded or is out of bounds.");
            return;
        }

        if (IsChunkUnloading(index))
        {
            Debug.LogError("Chunk at index " + index + " is already unloading! Check for curret unloading with Layer.IsUnloading(index).");
            return;
        }

        Chunk chunk = Chunks[index];

        unloading.Add(index);

        // TEST IO TODO FIXME
        ChunkIO.SaveChunk("James' Reality", Name, Tiles, chunk.X, chunk.Y, ChunkSize, ChunkSaved);        
    }

    public bool IsChunkUnloading(int index)
    {
        return unloading.Contains(index);
    }

    public bool IsChunkLoading(int index)
    {
        return loading.Contains(index);
    }

    private void ChunkSaved(object[] args)
    {
        // Remove chunk from world.

        // Get chunk position.
        int chunkX = (int)args[0];
        int chunkY = (int)args[1];

        // There are some more args that I don't really need for this.
        // Such as layer name (I already know!) and save path (I don't care -_-).

        int index = GetChunkIndex(chunkX, chunkY);

        if (!IsChunkLoaded(index))
        {
            Debug.LogError("Chunk was saved to memory, but physical copy could not be destroyed: it is not loaded!");
            return;
        }

        Chunk chunk = Chunks[index];

        int startX = chunk.X * ChunkSize;
        int startY = chunk.Y * ChunkSize;

        ClearTilesFrom(startX, startY, ChunkSize, ChunkSize);
        Chunks.Remove(index);
        Destroy(chunk.gameObject);

        unloading.Remove(index);
    }

    private void ClearTilesFrom(int startX, int startY, int sizeX, int sizeY)
    {
        for (int x = startX; x < startX + sizeX; x++)
        {
            for (int y = startY; y < startY + sizeY; y++)
            {
                Tiles[x][y] = null;
            }
        }
    }

    public Chunk GetChunkFromIndex(int index)
    {
        if (!IsChunkLoaded(index))
            return null;

        return Chunks[index];
    }

    public int GetChunkIndexFromTileCoords(int x, int y)
    {
        if(!InLayerBounds(x, y))
        {
            Debug.LogError("Outside of layer bounds! " + x + ", " + y);
            return -1;
        }

        int chunkX = (int)(x / ChunkSize);
        int chunkY = (int)(y / ChunkSize);

        return GetChunkIndex(chunkX, chunkY);
    }

    public Chunk GetChunkFromChunkCoords(int chunkX, int chunkY)
    {
        // Gets a chunk given the local chunk coordinates.
        if (!IsChunkInBounds(chunkX, chunkY))
        {
            Debug.LogError("Chunk coordiate " + chunkX + ", " + chunkY + " (chunk space) is not in bounds.");
            return null;
        }

        if (!IsChunkLoaded(chunkX, chunkY))
            return null;

        return Chunks[GetChunkIndex(chunkX, chunkY)];
    }

    public bool IsChunkLoaded(int x, int y)
    {
        // Is a chunk loaded?
        if (!IsChunkInBounds(x, y))
        {
            Debug.LogError("Chunk out of bounds!");
            return false;
        }

        return IsChunkLoaded(GetChunkIndex(x, y));
    }

    public bool IsChunkLoaded(int index)
    {
        // Is a chunk loaded?

        return Chunks.ContainsKey(index);
    }

    public bool IsChunkInBounds(int x, int y)
    {
        // Is a chunk coordinate in bounds?

        if (x < 0)
            return false;
        if (y < 0)
            return false;
        if (x >= WidthInChunks)
            return false;
        if (y >= HeightInChunks)
            return false;

        return true;
    }

    public int GetChunkIndex(int x, int y)
    {
        // Gets the unique index of a chunk.
        if (!IsChunkInBounds(x, y))
        {
            Debug.LogError("Chunk not in bounds, cannot get index!");
            return -1;
        }

        return x + (y * WidthInChunks);
    }

    public Vector2Int GetChunkCoordsFromIndex(int index)
    {
        // Gets the coordinates based on an index, note that it is chunk coordinates.
        return new Vector2Int(index % WidthInChunks, index / WidthInChunks);
    }

    public int CalculateChunksFromTiles(int sizeInTiles, int chunkSize)
    {
        // Calculates the chunks needed to hold that many tiles.
        // Return value of zero indicates an error.

        if (sizeInTiles <= 0)
            return 0;

        if (ChunkSize <= 0)
            return 0;

        int wholeTimes = sizeInTiles / ChunkSize;

        int remainder = sizeInTiles % ChunkSize;

        if (remainder != 0)
            wholeTimes++;

        return wholeTimes;
    }

    private List<int> indices = new List<int>();
    private List<int> toLoad = new List<int>();
    private List<int> toUnload = new List<int>();
    public void SelectiveLoad(int x, int y, int endX, int endY)
    {
        indices.Clear();
        toLoad.Clear();
        toUnload.Clear();

        for (int X = x; X < endX; X++)
        {
            for (int Y = y; Y < endY; Y++)
            {
                if (IsChunkInBounds(X, Y))
                {
                    indices.Add(GetChunkIndex(X, Y));
                }
            }
        }

        foreach(int index in Chunks.Keys)
        {
            // All currently loaded chunks, if NOT in list unload.
            if (!indices.Contains(index))
            {
                // Unload.
                toUnload.Add(index);
            }
        }

        foreach(int index in indices)
        {
            // if it loaded, do nothing.
            // If it is not loaded, load it.

            if(!IsChunkLoaded(index))
                toLoad.Add(index);
        }

        foreach(int index in toLoad)
        {
            // Load all of these.
            Vector2Int coordinates = GetChunkCoordsFromIndex(index);
            if(!IsChunkLoading(index))
                LoadChunk(coordinates.x, coordinates.y);
        }

        foreach(int index in toUnload)
        {
            // Unload these ones, if they are not already being unloaded.
            if(!IsChunkUnloading(index))
                UnloadChunk(index);
        }

        // Clean up.
        toUnload.Clear();
        toLoad.Clear();
        indices.Clear();
    }
}