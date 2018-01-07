using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(ActiveObject))]
public class Furniture : NetworkBehaviour
{
    // Furniture is a tile-like object that is placed within the world, that can have special behaviour,
    // can change its surroundings and manage chunk loading using an ActiveObject.

    public static Dictionary<string, Furniture> Loaded = new Dictionary<string, Furniture>();

    public string Prefab;
    public string Name;

    public string Layer = "Foreground";

    public int X { get; private set; }
    public int Y { get; private set; }
    public float Z = 0;

    [HideInInspector]
    public ActiveObject AO;

    private TileLayer layer;

    public void Awake()
    {
        AO = GetComponent<ActiveObject>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Register
        World.Instance.Furniture.RegisterFurniture(this, X, Y);
    }

    public override void OnNetworkDestroy()
    {
        base.OnNetworkDestroy();

        // Only on clients, which may be the host!
        World.Instance.Furniture.UnRegister(X, Y);
    }

    public TileLayer GetLayer()
    {
        if(layer == null)
        {
            layer = World.Instance.TileMap.GetLayer(Layer);
        }

        if(layer != null)
        {
            if(layer.Name != this.Layer)
            {
                layer = World.Instance.TileMap.GetLayer(this.Layer);
            }
        }

        return layer;
    }

    public void SetPosition(int x, int y)
    {
        if(GetLayer().InLayerBounds(x, y))
        {
            this.X = x;
            this.Y = y;
        }

        transform.position = new Vector3(X, Y, Z);
        name = Name + " (" + X + ", " + Y + ")";
    }

    public static void LoadAllFurniture()
    {
        if (Loaded != null)
        {
            Loaded.Clear();
        }
        else
        {
            Loaded = new Dictionary<string, Furniture>();
        }

        Furniture[] loaded = Resources.LoadAll<Furniture>("Furniture/");

        foreach (Furniture f in loaded)
        {
            Debug.Log("Loaded (" + f.Prefab + ") " + f.name);
            Loaded.Add(f.Prefab, f);
        }
    }

    public static void RegisterAll()
    {
        foreach (Furniture f in Loaded.Values)
        {
            NetworkManager.singleton.spawnPrefabs.Add(f.gameObject);
        }
    }

    public static bool FurnitureExists(string prefab)
    {
        return Loaded.ContainsKey(prefab);
    }

    public static Furniture GetFurniture(string prefab)
    {
        if (FurnitureExists(prefab))
        {
            return Loaded[prefab];
        }
        else
        {
            Debug.LogError("That furniture could not be found! '" + prefab + "'.");
            return null;
        }
    }
}