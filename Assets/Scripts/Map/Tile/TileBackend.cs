
using UnityEngine;
using UnityEngine.Networking;

public abstract class TileBackend : NetworkBehaviour
{
    // The network aware part of a tile.
    // Not much apart from networking.

    private int x, y;

    public abstract void ServerUpdate();
    public abstract void ClientUpdate();

    public virtual void PlacedClient()
    {

    }

    public virtual void RemovedClient()
    {

    }

    public virtual void PlacedServer()
    {

    }

    public virtual void RemovedServer()
    {

    }

    public int GetX()
    {
        return this.x;
    }

    public void GetY()
    {
        return this.y;
    }
}