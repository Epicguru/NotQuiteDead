using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PendingBuildingManager : MonoBehaviour
{
    // A class that handles pending outgoing tile/furniture placement requests.
    // Basically to fix the bug where more tiles would be placed than the client actually had.
    // Or where more items would be removed than were actaully placed.

    public static PendingBuildingManager Instance;

    public Dictionary<string, PendingBuildData> Pending = new Dictionary<string, PendingBuildData>();

    public void Awake()
    {
        Instance = this;
        Pending.Clear();	
    }

    public void OnDestroy()
    {
        Pending.Clear();
        Instance = null;
    }

    public bool IsPendingAt(int x, int y)
    {
        return Pending.ContainsKey(MakeID(x, y));
    }

    public PendingBuildData GetPendingAt(int x, int y)
    {
        if(!IsPendingAt(x, y))
        {
            Debug.LogError("Nothing pending @ {0}, {1}!".Form(x, y));
            return null;
        }

        return Pending[MakeID(x, y)];
    }

    public void AddPending(int x, int y, string prefab, BuildingItemType type, string layer)
    {
        if(IsPendingAt(x, y))
        {
            return;
        }

        PendingBuildData data = new PendingBuildData();
        data.Prefab = prefab;
        data.Type = type;
        data.Layer = layer;
        data.X = x;
        data.Y = y;

        Pending.Add(data.GetID(), data);

        // TODO: This can still mess up when the server cannot place a tile.
        // The tile or furniture should be returned to the client in that case.

        // TODO only remove once the tile is placed.
        // Now do the real placing request...
        switch (data.Type)
        {
            case BuildingItemType.TILE:

                // Place tile.
                BaseTile tile = BaseTile.GetTile(data.Prefab);
                World.Instance.TileMap.GetLayer(layer).SetTile(tile, x, y);

                break;
            case BuildingItemType.FURNITURE:

                // Place furniture
                World.Instance.Furniture.PlaceFurniture(data.Prefab, x, y);

                break;
        }
    }

    public void ConfirmPlaced(string id)
    {
        // TODO each request needs a unique Id based on client.

        // Remove from pending and remove tile from inventory...
        if (!Pending.ContainsKey(id))
            return; // That's fine, it's just not for us.

        PendingBuildData data = Pending[id];
        Player.Local.BuildingInventory.RemoveItems(data.Prefab, 1);
        Pending.Remove(id);
        Debug.Log("Confirm placed {0}. Removed {1} from inventory.".Form(id, data.Prefab));
    }

    public static string MakeID(int x, int y)
    {
        return x.ToString() + "." + y.ToString();
    }

    public void Update()
    {
        DebugText.Log("{0} pending tile/furniture placement requests.".Form(Pending.Count));
    }
}

public class PendingBuildData
{
    public BuildingItemType Type; // Tile or furniture?
    public string Prefab; // For both tiles and furnitures.
    public int X;
    public int Y;
    public string Layer;

    public string GetID()
    {
        return PendingBuildingManager.MakeID(X, Y);
    }
}