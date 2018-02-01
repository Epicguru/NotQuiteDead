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
    public bool Saving { get; private set; }
    public int ChunksLeftToSave { get; private set; }
    public int OperationsPendingInSave { get; private set; }
    public bool Use_RLE_In_Net = true;

    // Map of chunks, where key is the index. (The calculated index, x * height + y).
    public Dictionary<int, Chunk> Chunks = new Dictionary<int, Chunk>();
    public Dictionary<int, List<NetPendingTile>> PendingOperations = new Dictionary<int, List<NetPendingTile>>();
    private List<int> unloading = new List<int>();
    private List<int> loading = new List<int>();
    private BaseTile[][] Tiles;
    private float saveStartTime;

    public void Update()
    {
        int x = (int)InputManager.GetMousePos().x;
        int y = (int)InputManager.GetMousePos().y;

        if(CanPlaceTile(x, y))
        {
            if (Input.GetKey(KeyCode.Q))
            {
                SetTile(BaseTile.GetTile("Dirt"), x, y);
            }
            if (Input.GetKey(KeyCode.T))
            {
                SetTile(BaseTile.GetTile("Wood"), x, y);
            }            
            if (Input.GetKey(KeyCode.F))
            {
                World.Instance.Furniture.PlaceFurniture("Torch", x, y);
            }
            if (Input.GetKey(KeyCode.J))
            {
                World.Instance.Furniture.PlaceFurniture("Workbench", x, y);
            }
            if (Input.GetKey(KeyCode.G))
            {
                World.Instance.Furniture.PlaceFurniture("Turret", x, y);
            }            
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (isServer)
            {
                Furniture f = World.Instance.Furniture.GetFurnitureAt(x, y);
                if (f != null)
                {
                    Destroy(f.gameObject);
                }
            }
            if(CanPlaceTile(x, y))
                SetTile(null, x, y);
        }

        if (isServer && Input.GetKeyDown(KeyCode.H))
        {
            Pawn.SpawnPawn("Caveman", InputManager.GetMousePos());
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SaveAll();
        }

        if (!DebugText._Instance.Active)
            return;

        if (isServer)
        {
            int chunks = 0;
            int count = 0;
            foreach(int index in PendingOperations.Keys)
            {
                if (AnyPendingOperationsFor(index))
                    chunks++;
                else
                    continue;

                count += PendingOperations[index].Count;
            }
            DebugText.Log(count + " pending tile operations over " + chunks + " chunks.");

            if (Saving)
            {
                DebugText.Log("Saving '" + Name + "' ... ", Color.green);
                DebugText.Log("Chunks Pending: " + ChunksLeftToSave, Color.green);
                DebugText.Log("Tile Ops Pending: " + OperationsPendingInSave, Color.green);
            }
        }

        DebugText.Log(loading.Count + " chunks are loading.");
        DebugText.Log(unloading.Count + " chunks are unloading.");
    }

    [Server]
    public void SaveAll()
    {
        // Saves everything about this layer to file, ready to shut down.
        // Async, for now. You could block until Saving == false to have sync behaviour.

        if (Saving)
        {
            Debug.LogError("Already saving! " + ChunksLeftToSave + " chunks to go!");
            return;
        }

        Debug.Log("Saving layer '" + Name + "'");

        Saving = true;
        saveStartTime = Time.time;

        // Debug.
        int count = 0;
        foreach (int index in PendingOperations.Keys)
        {
            if (!AnyPendingOperationsFor(index))
                continue;

            count += PendingOperations[index].Count;
        }

        ChunksLeftToSave = 0;
        OperationsPendingInSave = count;

        // First save all loaded chunks.
        foreach(int index in Chunks.Keys)
        {
            Vector2Int pos = GetChunkCoordsFromIndex(index);
            ChunksLeftToSave++;
            ChunkIO.SaveChunk("James' Reality", Name, Tiles, pos.x, pos.y, ChunkSize, ChunkSavedEmpty);
        }

        // Tile operation merging will be done once all loaded chunks have been saved to file.

        // Done!
    }

    [Server]
    private void ChunkSavedEmpty(object[] args)
    {
        // Detects when all chunks have been saved.
        ChunksLeftToSave--;
        if(ChunksLeftToSave == 0)
        {
            // Then merge all pending operation to file.
            ResolveAllPendingOperations();
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

    public BaseTile Unsafe_GetTile(int x, int y)
    {
        if(InLayerBounds(x, y))
        {
            return Tiles[x][y];
        }
        else
        {
            return null;
        }
    }

    public bool IsSpotWalkable(int x, int y)
    {
        if (!InLayerBounds(x, y))
            return false;

        bool tileWalkable = true;
        BaseTile tile = Tiles[x][y];
        if(tile != null)
        {
            if (tile.Walkable == false)
                tileWalkable = false;
        }

        bool furnitureWalkable = true;
        Furniture f = World.Instance.Furniture.GetFurnitureAt(x, y);
        if(f != null)
        {
            if (!f.Walkable)
                furnitureWalkable = false;
        }

        return tileWalkable && furnitureWalkable;
    }

    /// <summary>
    /// Places a tile, can be called on client or server from the main thread.
    /// Has widely varying implementation depending on the situation and server status.
    /// However, the goal is always the same: place a tile at that position as soon as possible. It may be a few frames later though!
    /// 
    /// Note that:
    ///  -If called on client, the chunk must be loaded for this to succeed.
    ///  -If called on server, the chunk must be loaded for this to succeed.
    /// </summary>
    /// <param name="tile">The tile prefab. See BaseTile.Get().</param>
    /// <param name="x">The x position, in tiles.</param>
    /// <param name="y">The y position, in tiles.</param>
    /// <returns>True if the operation was successful, but keep in mind that the tile may not be placed instantly if called from the client.</returns>
    public bool SetTile(BaseTile tile, int x, int y)
    {
        if (isServer)
        {
            return SetTile_Server(tile, x, y);
        }
        else
        {
            return SetTile_Client(tile, x, y);
        }
    }

    [Server]
    private bool SetTile_Server(BaseTile tile, int x, int y, bool network = true)
    {
        if (!CanPlaceTile(x, y))
        {
            Debug.LogError("Cannot place tile at " + x + ", " + y + " in layer '" + Name + "'. (SERVER). Must be loaded, in bounds.");
            return false;
        }

        // For now, just place tile in position.
        BaseTile oldTile = Tiles[x][y];

        if (oldTile == tile)
        {
            // No need to change anything, already set that tile there!
            return false;
        }

        Chunk c = GetChunkFromIndex(GetChunkIndexFromTileCoords(x, y));

        if (oldTile != null)
        {
            TileRemoved(x, y, oldTile, c);
        }

        // Apply tile.
        Tiles[x][y] = tile;

        // Update tiles sourrounding and update physics bodies.
        TilePlaced(x, y, tile, c);

        // Send to clients.
        if(network)
            NetworkServer.SendToAll((short)MessageTypes.SEND_TILE_CHANGE, new Msg_SendTile() { Prefab = (tile == null ? null : tile.Prefab), X = x, Y = y, Layer = Name });

        return true;
    }

    [Client]
    private bool SetTile_Client(BaseTile tile, int x, int y)
    {
        // On a client, we need to request that the tile is placed on the server. How long it will take until the tile is placed it unknown.

        if(GetTile(x, y) != tile)
        {
            Player.Local.NetUtils.CmdRequestTileChange(tile == null ? null : tile.Prefab, x, y, Name);
            return true;
        }
        else
        {
            return false;
        }
    }

    [Server]
    public void ClientRequestingTileChange(BaseTile tile, int x, int y)
    {
        // Called on the server when a client has requested to change a tile.

        // No validation in terms of bounds, would be too slow.
        
        // 1. Is chunk loaded?
        if(IsChunkLoaded(GetChunkIndexFromTileCoords(x, y)))
        {
            // Apply tile and then send changes to clients.

            // Normal tile setting.
            BaseTile oldTile = Tiles[x][y];

            if (oldTile == tile)
            {
                // No need to change anything, already set that tile there!
                // Silly client!
                return;
            }

            Chunk c = GetChunkFromIndex(GetChunkIndexFromTileCoords(x, y));

            if (oldTile != null)
            {
                TileRemoved(x, y, oldTile, c);
            }

            // Apply tile.
            Tiles[x][y] = tile;

            // Update tiles sourrounding and update physics bodies.
            TilePlaced(x, y, tile, c);

            // Send to clients.
            NetworkServer.SendToAll((short)MessageTypes.SEND_TILE_CHANGE, new Msg_SendTile() { Prefab = (tile == null ? null : tile.Prefab), X = x, Y = y, Layer = Name });
        }
        else
        {
            // Chunk is not loaded! Save the changes and send to clients.

            // Add a pending tile operation.
            AddPendingTileOperation(tile == null ? null : tile.Prefab, x, y);

            // Send to any clients that are in that chunk, including the one that requested this change.
            NetworkServer.SendToAll((short)MessageTypes.SEND_TILE_CHANGE, new Msg_SendTile() { Prefab = (tile == null ? null : tile.Prefab), X = x, Y = y, Layer = Name });
        }
    }

    [Client]
    public void RecievedTileChange(Msg_SendTile data)
    {
        // Called on clients when a tile has been set.

        // If we are a host, just stop. Tile has already been set.
        if (isServer) 
            return;

        // Check if is in bounds.
        if(!InLayerBounds(data.X, data.Y))
        {
            Debug.LogError("Why did the server send me coordinates out of bounds?!?! (" + data.X + ", " + data.Y + ")");
            return;
        }

        // Check if it is loaded. If it is not ignore this. TODO if it is loading, make it be applied once loaded.
        if(IsChunkLoaded(GetChunkIndexFromTileCoords(data.X, data.Y)))
        {
            // Place tile in position.
            BaseTile oldTile = Tiles[data.X][data.Y];

            BaseTile tile = data.Prefab == null ? null : BaseTile.GetTile(data.Prefab);

            Chunk c = GetChunkFromIndex(GetChunkIndexFromTileCoords(data.X, data.Y));

            if (oldTile != null)
            {
                TileRemoved(data.X, data.Y, oldTile, c);
            }

            // Apply tile.
            Tiles[data.X][data.Y] = tile;

            // Update tiles sourrounding and update physics bodies.
            TilePlaced(data.X, data.Y, tile, c);
        }
    }

    private void SetChunkTiles(BaseTile[][] tiles, int x, int y)
    {
        if (tiles == null)
            return;

        // Intended for use when loading chunks, and nothing else.
        // Not networked at all.

        for (int X = x; X < x + tiles.Length; X++)
        {
            BaseTile[] yArray = tiles[X - x];

            for (int Y = y; Y < y + yArray.Length; Y++)
            {
                if(CanPlaceTile(X, Y))
                {
                    BaseTile tile = yArray[Y - y];

                    // For now, just place tile in position.
                    BaseTile oldTile = Tiles[X][Y];

                    Chunk c = GetChunkFromIndex(GetChunkIndexFromTileCoords(X, Y));

                    if (oldTile != null)
                    {
                        TileRemoved(X, Y, oldTile, c);
                    }

                    // Apply tile.
                    Tiles[X][Y] = tile;

                    // Update tiles sourrounding and update physics bodies.
                    TilePlaced(X, Y, tile, c);
                }
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

        if(FurnitureAt(x, y))
        {
            return false;
        }

        return true;
    }

    public bool FurnitureAt(int x, int y)
    {
        return World.Instance.Furniture.IsFurnitureAt(x, y);
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
        // TODO, pool chunks.

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
        Player.Local.NetUtils.CmdRequestChunk(Player.Local.gameObject, x, y, Name);
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
            ChunkIO.GetChunkForNet_Loaded(player, Tiles, x, y, ChunkSize, NetChunkMade, Use_RLE_In_Net);
        }
        else
        {
            // Chunk is not loaded, what do I do?
            // Load a copy and send it?
            // But then how does the client make persistent changes to it?
            // Load it permamently in the server while the client is in it?
            // Allow the client to send a copy back once it is unloaded?
            // ---> Allow the client to make changes, record them on the server, and once the chunk is loaded, apply those changes. Also save those changes when X?

            // Decided solution:
            // Load from file.
            // Apply any 'changes' (explained later).
            // Send to client.
            // Client can then make any changes to that chunk, which are sent to the server, and stored as 'changes'.
            // Once the server really loads the chunk, the 'changes' are applied for real.
            // When a chunk is saved to file, all pending changes to that chunk are removed.

            // 1. Ensure it is saved to file...
            if(ChunkIO.IsChunkSaved("James' Reality", Name, x, y))
            {
                // 2. Load from file.
                ChunkIO.GetChunkForNet_Unloaded(player, "James' Reality", Name, x, y, ChunkSize, NetChunkLoadedForClient, NetChunkLoadError);
            }
            else
            {
                // 3. 'Generate' and save to file, then send normally.
                string contents = (16 * 16) + "|?";
                string path = ChunkIO.GetPathForChunk("James' Reality", Name, x, y);
                File.WriteAllText(path, contents);

                // 4. Send it.
                ChunkIO.GetChunkForNet_Unloaded(player, "James' Reality", Name, x, y, ChunkSize, NetChunkLoadedForClient, NetChunkLoadError);
            }
        }
    }

    [Server]
    private void NetChunkLoadedForClient(object[] args)
    {
        // Called on server when finished loading a chunk from file, to send to a player.
        string data = (string)args[0];
        int chunkX = (int)args[1];
        int chunkY = (int)args[2];
        GameObject player = (GameObject)args[3];

        int index = GetChunkIndex(chunkX, chunkY);
        if (AnyPendingOperationsFor(index))
        {
            // Merge the loaded data with any pending operations.
            string merged = ChunkIO.Merge(data, PendingOperations[index], ChunkSize, true);

            // Compress again using RLE.
            float eff;
            string compressed = ChunkIO.RLE(merged, out eff);

            // Apply back to the data.
            data = compressed;
        }

        // Send to client.
        Msg_SendChunk msg = new Msg_SendChunk() { Data = data, ChunkX = chunkX, ChunkY = chunkY, Layer = Name };

        // Send the data to the client using the server.
        NetworkServer.SendToClientOfPlayer(player, (short)MessageTypes.SEND_CHUNK_DATA, msg);
    }

    [Server]
    private void NetChunkLoadError(string error)
    {
        // There has been an error when loading a chunk from file for a client.
        Debug.LogError("[NET CHUNK LOAD] " + error);
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

        Msg_SendChunk msg = new Msg_SendChunk() { Data = data, ChunkX = chunkX, ChunkY = chunkY, Layer = Name };

        // Send the data to the client using the server.
        NetworkServer.SendToClientOfPlayer(player, (short)MessageTypes.SEND_CHUNK_DATA, msg);
    }

    [Client]
    public void RecievedNetChunk(string data, int chunkX, int chunkY)
    {
        // Called on only clients, when the network message for an incomming chunk has been recieved.

        // Make sure that the chunk is created first!
        int index = GetChunkIndex(chunkX, chunkY);
        if (!IsChunkLoading(index))
        {
            return;
        }

        // First get a list of tiles, based on the data.
        BaseTile[][] tiles = ChunkIO.MakeChunk(data, ChunkSize, null, Use_RLE_In_Net);

        SetChunkTiles(tiles, chunkX * ChunkSize, chunkY * ChunkSize);


        // Mark as done loading.
        Chunk c = Chunks[index];
        c.DoneLoading();

        loading.Remove(index);
    }

    [Server]
    private void ChunkLoaded(object[] args)
    {
        // Called on server when the chunk has been loaded from file.

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

        SetChunkTiles(tiles, chunkX * ChunkSize, chunkY * ChunkSize);

        // Apply pending operations, if there are any.
        ApplyPendingOperationsToChunk(index);

        Chunk c = Chunks[index];
        c.DoneLoading();

        loading.Remove(index);
    }

    [Server]
    private void ChunkLoadError(string error)
    {
        Debug.LogError("[CHUNK LOAD] " + error);
    }

    public void UnloadChunk(int x, int y)
    {
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

        if (isServer)
        {
            UnloadChunk_Server(index, chunk);
        }
        else
        {
            UnloadChunk_Client(index, chunk);
        }
    }

    [Server]
    private void UnloadChunk_Server(int index, Chunk chunk)
    {
        unloading.Add(index);
        ChunkIO.SaveChunk("James' Reality", Name, Tiles, chunk.X, chunk.Y, ChunkSize, ChunkSaved);
    }

    [Client]
    private void UnloadChunk_Client(int index, Chunk chunk)
    {
        unloading.Add(index);
        // Don't save to file, just destroy using the callback function.
        ChunkSaved(new object[] { chunk.X, chunk.Y });
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

    [Server]
    public bool AnyPendingOperationsFor(int index)
    {
        return PendingOperations.ContainsKey(index) ? PendingOperations[index] != null && PendingOperations[index].Count > 0 : false;
    }

    [Server]
    public void AddPendingTileOperation(string prefab, int x, int y)
    {
        // X and Y are in global space.
        // Add a pending operation. If there is already one in existence, it will overwrite the last one. Does not replace it.

        int index = GetChunkIndexFromTileCoords(x, y);
        Vector2Int chunkPos = GetChunkCoordsFromIndex(index);

        if (PendingOperations.ContainsKey(index))
        {
            if(PendingOperations[index] == null)
            {
                // Should not be, but might as well check.
                PendingOperations[index] = new List<NetPendingTile>();
            }
        }
        else
        {
            PendingOperations.Add(index, new List<NetPendingTile>());
        }

        PendingOperations[index].Add(new NetPendingTile() { Prefab = prefab, X = x - chunkPos.x * ChunkSize, Y = y - chunkPos.y * ChunkSize });
    }

    [Server]
    public void ResolveAllPendingOperations()
    {
        // Saves all pending operations to file and clears the pending array.
        ChunkIO.MergeAllToFile("James' Reality", Name, ChunkSize, WidthInChunks, PendingOperations, OperationsResolved);

        // The operations will be cleared once they have been saved.
    }

    [Server]
    private void OperationsResolved(object[] args)
    {
        //Debug.Log("All pending tile operations merged to file.");

        // Now clear the pending operations.
        PendingOperations.Clear();

        if (Saving)
        {
            OperationsPendingInSave = 0;
            Saving = false;
            Debug.Log("Finished saving '" + Name + "'. Took " + (Time.time - saveStartTime) + " seconds.");
        }
    }

    [Server]
    public void ApplyPendingOperationsToChunk(int index, bool destroyList = false)
    {
        // Applies pending tile operations from clients to a loaded chunk on the server.
        if (AnyPendingOperationsFor(index))
        {
            if (!IsChunkLoaded(index))
            {
                if (!IsChunkLoading(index))
                {
                    Debug.LogError("The chunk for index " + index + " is not loaded or loading, cannot apply pending operations.");
                }
            }

            Chunk chunk = GetChunkFromIndex(index);

            // Set tiles based on the pending tile operations.
            foreach(NetPendingTile op in PendingOperations[index])
            {
                bool empty = op.PrefabIsNull();
                BaseTile tile = null;
                if (!empty)
                {
                    if (!BaseTile.ContainsTile(op.Prefab))
                    {
                        Debug.LogError("Pending operation requested tile '" + op.Prefab + "', but the server could not find it.");
                        continue;
                    }

                    tile = BaseTile.GetTile(op.Prefab);
                }

                int globalX = op.X + chunk.X * ChunkSize;
                int globalY = op.Y + chunk.Y * ChunkSize;

                // Set the tile, locally on the server, without networking.
                SetTile_Server(tile, globalX, globalY, false);
            }

            // Clear those pending operations, they are no longer pending!
            if (!destroyList)
                PendingOperations[index].Clear();
            else
                PendingOperations.Remove(index);
        }
    }
}