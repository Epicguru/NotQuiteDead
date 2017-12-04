using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileLayer : MonoBehaviour
{
    public Chunk ChunkPrefab;
    public int ChunkSize = 16;

    public string Name { get; private set; }

    public int Width { get; private set; }
    public int Height { get; private set; }
    public int WidthInChunks { get; private set; }
    public int HeightInChunks { get; private set; }

    // Map of chunks, where key is the index. (The calculated index, x * height + y).
    public Dictionary<int, Chunk> Chunks = new Dictionary<int, Chunk>();

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

        this.WidthInChunks = CalculateChunksFromTiles(Width, ChunkSize);
        this.HeightInChunks = CalculateChunksFromTiles(Height, ChunkSize);

        Debug.Log("Using a chunk size of " + ChunkSize + ", there are " + WidthInChunks + " horizontal chunks and " + HeightInChunks + " vertical chunks.");
        Debug.Log("Real size " + ChunkSize * WidthInChunks * HeightInChunks + " tiles, " + ChunkSize * WidthInChunks + "x" + ChunkSize * HeightInChunks + ".");
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

        Chunk newChunk = Instantiate(ChunkPrefab.gameObject, transform).GetComponent<Chunk>();
        int index = GetChunkIndex(x, y);

        newChunk.Create(x, y, ChunkSize, ChunkSize, index);

        Chunks.Add(index, newChunk);
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
        Chunk chunk = Chunks[index];
        Chunks.Remove(index);
        Destroy(chunk.gameObject);
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
            LoadChunk(coordinates.x, coordinates.y);
        }

        foreach(int index in toUnload)
        {
            // Unload these ones.
            UnloadChunk(index);
        }

        // Clean up.
        toUnload.Clear();
        toLoad.Clear();
        indices.Clear();
    }
}