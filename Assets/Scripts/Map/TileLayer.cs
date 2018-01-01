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
    public bool Use_RLE_In_Net = true;

    // Map of chunks, where key is the index. (The calculated index, x * height + y).
    public Dictionary<int, Chunk> Chunks = new Dictionary<int, Chunk>();
    private List<int> unloading = new List<int>();
    private List<int> loading = new List<int>();
    private BaseTile[][] Tiles;

    public void Update()
    {
        int x = (int)InputManager.GetMousePos().x;
        int y = (int)InputManager.GetMousePos().y;

        if(CanPlaceTile(x, y))
        {
            if (InputManager.InputPressed("Shoot"))
            {
                BaseTile place = InputManager.InputPressed("Sprint") ? null : BaseTile.GetTile("Dirt");
                SetTile(place, x, y);
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

        Chunk c = GetChunkFromIndex(GetChunkIndexFromTileCoords(x, y));

        if(oldTile != null)
        {
            TileRemoved(x, y, oldTile, c);
        }

        Tiles[x][y] = tile;

        TilePlaced(x, y, tile, c);

        return true;
    }

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

    private void TilePlaced(int x, int y, BaseTile newTile, Chunk chunk)
    {
        if (chunk == null)
        {
            Debug.LogError("TilePlaced called in unloaded chunk!");
            return;
        }

        int localX = x - (ChunkSize * chunk.X);
        int localY = y - (ChunkSize * chunk.Y);

        // Set physics if the tile is not null.
        if (newTile != null && newTile.Physics)
        {
            chunk.Physics.AssignCollider(newTile.Physics, localX, localY);
        }

        // Works if it is null or not.
        chunk.Texture.SetTile(newTile, this, x, y, localX, localY);

        UpdateSurroundingTextures(x, y);
    }

    private void UpdateSurroundingTextures(int x, int y)
    {
        // Update the textures of the surrounding tiles now that a new tile has been set.

        int X = 0;
        int Y = 0;
        BaseTile tile = null;

        // Left
        X = x + -1;
        Y = y + 0;
        tile = GetTile(X, Y);
        if (tile != null)
        {
            Chunk chunk = GetChunkFromIndex(GetChunkIndexFromTileCoords(X, Y));
            chunk.Texture.SetTile(tile, this, X, Y, X - (ChunkSize * chunk.X), Y - (ChunkSize * chunk.Y));
        }

        // Top
        X = x + 0;
        Y = y + 1;
        tile = GetTile(X, Y);
        if (tile != null)
        {
            Chunk chunk = GetChunkFromIndex(GetChunkIndexFromTileCoords(X, Y));
            chunk.Texture.SetTile(tile, this, X, Y, X - (ChunkSize * chunk.X), Y - (ChunkSize * chunk.Y));
        }

        // Right
        X = x + 1;
        Y = y + 0;
        tile = GetTile(X, Y);
        if (tile != null)
        {
            Chunk chunk = GetChunkFromIndex(GetChunkIndexFromTileCoords(X, Y));
            chunk.Texture.SetTile(tile, this, X, Y, X - (ChunkSize * chunk.X), Y - (ChunkSize * chunk.Y));
        }

        // Bottom
        X = x + 0;
        Y = y + -1;
        tile = GetTile(X, Y);
        if (tile != null)
        {
            Chunk chunk = GetChunkFromIndex(GetChunkIndexFromTileCoords(X, Y));
            chunk.Texture.SetTile(tile, this, X, Y, X - (ChunkSize * chunk.X), Y - (ChunkSize * chunk.Y));
        }
    }

    public void TileRemoved(int x, int y, BaseTile oldTile, Chunk chunk)
    {
        if(chunk == null)
        {
            Debug.LogError("TileRemoved called in unloaded chunk!");
            return;
        }

        // Remove physics collider if it had one.
        if(oldTile != null)
        {
            int localX = x - chunk.X * ChunkSize;
            int localY = y - chunk.Y * ChunkSize;

            // Null-safe.
            chunk.Physics.RemoveCollider(localX, localY);
        }
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
        // TODO, pool chunks. EDIT - Wait for unity job system.
        // This would load from file. EDIT - Done

        if (isServer)
        {
            LoadChunk_Server(x, y);
        }
        else
        {
            LoadChunk_Client(x, y);
        }        
    }

    [Server]
    private void LoadChunk_Server(int x, int y)
    {
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
        if (ChunkIO.IsChunkSaved("James' Reality", Name, x, y))
        {
            // Load from disk.
            ChunkIO.LoadChunk("James' Reality", Name, x, y, ChunkSize, ChunkLoaded, ChunkLoadError);
        }
        else
        {
            // TODO add something, like generating this chunk.
            // Just leave a blank chunk, lol.

            // Mark as loaded, after 'generation'.
            Chunks[index].DoneLoading();
            loading.Remove(index);
        }
    }

    [Client]
    private void LoadChunk_Client(int x, int y)
    {
        RequestChunkFromServer(x, y);
    }

    [Client]
    public void RequestChunkFromServer(int x, int y)
    {
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
            Debug.LogError("Chunk at " + x + ", " + y + " is already loading (request from server).");
            return;
        }

        // Flag as loading, even though it is really being sent from the server.
        loading.Add(index);

        Chunk newChunk = Instantiate(ChunkPrefab.gameObject, transform).GetComponent<Chunk>();

        // Create the chunk object.
        newChunk.Create(x, y, ChunkSize, ChunkSize, index);

        // Add to the chunks list.
        Chunks.Add(index, newChunk);

        // Now request from the server...
        Player.Local.NetUtils.CmdRequestChunk(Player.Local.gameObject, x, y);
    }

    [Server] // Intentional, called from Player.NetUtils.
    public void CmdRequestChunk(int x, int y, GameObject player)
    {
        // Called when a player requests a chunk from this server.
        // We need to make a compressed representation of the tiles and send it.

        // First, validation.
        if (player == null)
            return;

        if (!IsChunkInBounds(x, y))
            return;

        // FOR NOW:

        // 1. See if chunk is loaded.
        if(IsChunkLoaded(x, y))
        {
            // Great, send a copy of this.
            ChunkIO.GetChunkForNet(player, Tiles, x, y, ChunkSize, NetChunkMade, Use_RLE_In_Net);
        }
        else
        {
            // Chunk is not loaded, what do I do?
            // Load a copy and send it?
            // But then how does the client make persistent changes to it?
            // Load it permamently in the server while the client is in it?
            // Allow the client to send a copy back once it is unloaded?
            // ---> Allow the client to make changes, record them on the server, and once the chunk is loaded, apply those changes. Also save those changes when X?
        }
    }

    [Server]
    private void NetChunkMade(object[] args)
    {
        // Called when a player requests a chunk from the server, then the server posts an event that leads here.
        bool worked = (bool)args[0];

        if (!worked)
        {
            Debug.LogError("Net chunk creation failed, unknown error.");
            return;
        }

        string data = (string)args[1];
        int chunkX = (int)args[2];
        int chunkY = (int)args[3];
        GameObject player = (GameObject)args[4];

        Msg_SendChunk msg = new Msg_SendChunk() { Data = data, ChunkX = chunkX, ChunkY = chunkY };

        // Send the data to the client using the server.
        NetworkServer.SendToClientOfPlayer(player, (short)MessageTypes.SEND_CHUNK_DATA, msg);
    }

    [Client]
    public void RecievedNetChunk(string data, int chunkX, int chunkY)
    {
        // Called on only clients, when the network message for an incomming chunk has been recieved.

        // First get a list of tiles, based on the data.
        BaseTile[][] tiles = ChunkIO.MakeChunk(data, ChunkSize, null, Use_RLE_In_Net);

        SetTiles(tiles, chunkX * ChunkSize, chunkY * ChunkSize);

        int index = GetChunkIndex(chunkX, chunkY);

        Chunk c = Chunks[index];
        c.DoneLoading();

        loading.Remove(index);
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

        Chunk c = Chunks[index];
        c.DoneLoading();

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