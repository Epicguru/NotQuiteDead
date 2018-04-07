using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActiveObject : MonoBehaviour {

    // Represents an active object, which requires one or more chunks to loaded around it.

    public List<int> RequestedChunks = new List<int>();
    public string DefaultLayer = "Foreground";

    private TileLayer defaultTilelayer;

    public void Awake()
    {
        ActiveObjectManager.Instance.Objects.Add(this);
    }

    public void OnDestroy()
    {
        ActiveObjectManager.Instance.Objects.Remove(this);
    }

    private TileLayer GetDefaultTilelayer()
    {
        if(defaultTilelayer == null)
        {
            defaultTilelayer = World.Instance.TileMap.GetLayer(DefaultLayer);
        }

        return defaultTilelayer;
    }

    /// <summary>
    /// Resquests that the chunk that contains that tile coordinate is loaded.
    /// </summary>
    /// <param name="tileX">The x position of the tile.</param>
    /// <param name="tileY">The y position of the tile.</param>
    public void RequestChunkFromTile(int tileX, int tileY)
    {
        if(GetDefaultTilelayer().InLayerBounds(tileX, tileY))
        {
            this.RequestChunk(GetDefaultTilelayer().GetChunkIndexFromTileCoords(tileX, tileY));
        }
    }

    /// <summary>
    /// Requests for the chunk at the coodinates to be loaded. In chunk space, not tile space.
    /// </summary>
    /// <param name="x">The x position of the chunk.</param>
    /// <param name="y">The y position of the chunk.</param>
    public void RequestChunk(int x, int y)
    {
        if(GetDefaultTilelayer().IsChunkInBounds(x, y))
        {
            this.RequestChunk(GetDefaultTilelayer().GetChunkIndex(x, y));
        }
    }

    /// <summary>
    /// Requests that the chunk of the index be loaded.
    /// </summary>
    /// <param name="index">The index of the chunk.</param>
    public virtual void RequestChunk(int index)
    {
        if (RequestedChunks.Contains(index))
            return;

        RequestedChunks.Add(index);
    }

    public void CancelAllRequests()
    {
        RequestedChunks.Clear();
    }

    public void CancelRequest(int index)
    {
        if (!RequestedChunks.Contains(index))
            return;

        RequestedChunks.Remove(index);
    }

    public void CancelRequest(int x, int y)
    {
        if (GetDefaultTilelayer().IsChunkInBounds(x, y))
        {
            this.CancelRequest(GetDefaultTilelayer().GetChunkIndex(x, y));
        }
    }

    public void CancelRequestFromTile(int tileX, int tileY)
    {
        if (GetDefaultTilelayer().InLayerBounds(tileX, tileY))
        {
            this.CancelRequest(GetDefaultTilelayer().GetChunkIndexFromTileCoords(tileX, tileY));
        }
    }
}