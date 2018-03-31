using System.Collections.Generic;
using System.Linq;
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

    public Furniture[] GetAllFurniture()
    {
        return furniture.Values.ToArray();
    }

    public bool PlaceFurniture(string prefab, int x, int y)
    {
        if (isServer)
        {
            return Place_Server(prefab, x, y);
        }
        else
        {
            return Place_Client(prefab, x, y);
        }
    }

    [Server]
    private bool Place_Server(string prefab, int x, int y)
    {
        if(prefab == null)
        {
            if(IsFurnitureAt(x, y))
            {
                Furniture placed = GetFurnitureAt(x, y, true);

                // Will destroy on clients.
                Destroy(placed);

                // Sucessfully placed...
                return true;
            }
            return true;
        }
        else
        {
            if(IsFurnitureAt(x, y))
            {
                // Can't just place where another furniture is...
                // Destroy it first.
                return false;
            }

            if(!GetLayer().InLayerBounds(x, y))
            {
                return false;
            }

            if (TileAt(x, y))
            {
                return false;
            }

            Furniture pre = Furniture.GetFurniture(prefab);
            if(pre != null)
            {
                Furniture instance = Instantiate(pre.gameObject).GetComponent<Furniture>();

                instance.SetPosition(x, y);

                NetworkServer.Spawn(instance.gameObject);
                return true;
            }
            return false;
        }
    }

    [Client]
    private bool Place_Client(string prefab, int x, int y)
    {
        if (Player.Local == null)
            return false;

        if(!GetLayer().InLayerBounds(x, y))
        {
            Debug.LogError("Not in bounds! (" + x + ", " + y + ")");
            return false;
        }

        if(IsFurnitureAt(x, y))
        {
            Debug.LogError("Already furniture there! (" + x + ", " + y + ")");
            return false;
        }

        // TODO furniture on top of tiles?
        if(TileAt(x, y))
        {
            Debug.LogError("Tile is at that position. Cannot have furniture and tile in the same place!");
            return false;
        }

        Player.Local.NetUtils.CmdPlaceFurniture(prefab, prefab == null, x, y);
        return true;
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
            Debug.LogWarning("Already something at {0}, {1}".Form(x, y));
            return;
        }

        // Set parent
        placed.transform.SetParent(Parent);

        // Register furniture at that position.
        furniture.Add(GetIndexAt(x, y), placed);

        // Confirm to pending building system.
        PendingBuildingManager.Instance.ConfirmPlaced(PendingBuildingManager.MakeID(x, y));
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