using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FurnitureManager : NetworkBehaviour
{
    public string Layer = "Foreground";

    public Transform Parent;

    private Dictionary<int, Furniture> furniture = new Dictionary<int, Furniture>();
    private TileLayer layer;

    public TileLayer GetLayer()
    {
        if (layer == null)
            layer = World.Instance.TileMap.GetLayer(Layer);

        return layer;
    }

    public void Update()
    {
        DebugText.Log(furniture.Count + " furniture placed throughout the world.");
    }

    public void PlaceFurniture(string prefab, int x, int y)
    {
        if (isServer)
        {
            Place_Server(prefab, x, y);
        }
        else
        {
            Place_Client(prefab, x, y);
        }
    }

    [Server]
    private void Place_Server(string prefab, int x, int y)
    {
        if(prefab == null)
        {
            if(IsFurnitureAt(x, y))
            {
                Furniture placed = GetFurnitureAt(x, y, true);

                // Will destroy on clients.
                Destroy(placed);
            }
        }
        else
        {
            if(IsFurnitureAt(x, y))
            {
                return;
            }

            if(!GetLayer().InLayerBounds(x, y))
            {
                return;
            }

            if (TileAt(x, y))
            {
                return;
            }

            Furniture pre = Furniture.GetFurniture(prefab);
            if(pre != null)
            {
                Furniture instance = Instantiate(pre.gameObject).GetComponent<Furniture>();

                instance.SetPosition(x, y);

                NetworkServer.Spawn(instance.gameObject);
            }
        }
    }

    [Client]
    private void Place_Client(string prefab, int x, int y)
    {
        if (Player.Local == null)
            return;

        if(!GetLayer().InLayerBounds(x, y))
        {
            Debug.LogError("Not in bounds! (" + x + ", " + y + ")");
            return;
        }

        if(IsFurnitureAt(x, y))
        {
            Debug.LogError("Already furniture there! (" + x + ", " + y + ")");
            return;
        }

        // TODO furniture on top of tiles?
        if(TileAt(x, y))
        {
            Debug.LogError("Tile is at that position. Cannot have furniture and tile in the same place!");
            return;
        }

        Player.Local.NetUtils.CmdPlaceFurniture(prefab, prefab == null, x, y);
    }

    public bool TileAt(int x, int y)
    {
        return GetLayer().GetTile(x, y) != null;
    }

    [Server]
    public void RequestingPlace(string prefab, int x, int y)
    {
        // Already validated, but check for existing furniture.
        if(IsFurnitureAt(x, y))
        {
            return;
        }

        PlaceFurniture(prefab, x, y); // Will call the server version.
    }

    public void RegisterFurniture(Furniture placed, int x, int y)
    {
        // Called on both client and servers when the object is spawned.
        if(IsFurnitureAt(x, y))
        {
            return;
        }

        placed.transform.SetParent(Parent);

        furniture.Add(GetIndexAt(x, y), placed);
    }

    public void UnRegister(int x, int y)
    {
        if(!IsFurnitureAt(x, y))
        {
            Debug.LogError("No furniture tile at (" + x + ", " + y + ")");
            return;
        }

        furniture.Remove(GetIndexAt(x, y));
    }

    public bool IsFurnitureAt(int x, int y)
    {
        int index = GetIndexAt(x, y);

        return furniture.ContainsKey(index);
    }

    public Furniture GetFurnitureAt(int x, int y, bool skipCheck = false)
    {
        if (!skipCheck)
        {
            if(!IsFurnitureAt(x, y))
            {
                return null;
            }
        }

        return furniture[GetIndexAt(x, y)];
    }

    public int GetIndexAt(int x, int y)
    {
        int width = GetLayer().Width;

        return x + (y * width);
    }
}