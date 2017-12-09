
using UnityEngine;
using UnityEngine.Networking;

public abstract class TileBackend : NetworkBehaviour
{
    // The network aware part of a tile.
    // Not much apart from networking.

    private int x, y;
    private bool placed;

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