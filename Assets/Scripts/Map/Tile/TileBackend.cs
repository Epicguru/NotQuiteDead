
using UnityEngine;
using UnityEngine.Networking;

public abstract class TileBackend : NetworkBehaviour
{
    // The network aware part of a tile.
    // Not much apart from networking.

    public bool IsReference
    {
        get
        {
            return realTile != null;
        }
    }

    private int x, y;
    private bool placed;
    private TileBackend realTile;

    // SERIOUSLY, CALL base.Update()!
    public virtual void Update()
    {
        // Call the other methods.
        if (placed)
        {
            if (isServer)
            {
                UpdateServer();
                UpdateClient();
            }
            else
            {
                UpdateClient();
            }
        }
    }

    public abstract void UpdateServer();
    public abstract void UpdateClient();

    public void SetRealTile(TileBackend realTile)
    {
        this.realTile = realTile;
    }

    public virtual void PlacedClient()
    {
        placed = true;
    }

    public virtual void RemovedClient()
    {
        placed = false;
    }

    public virtual void PlacedServer()
    {
        placed = true;
    }

    public virtual void RemovedServer()
    {
        placed = false;
    }

    public int GetX()
    {
        return this.x;
    }

    public int GetY()
    {
        return this.y;
    }
}