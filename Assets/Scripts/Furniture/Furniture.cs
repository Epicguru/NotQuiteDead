using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(ActiveObject))]
public class Furniture : NetworkBehaviour
{
    // Furniture is a tile-like object that is placed within the world, that can have special behaviour,
    // can change its surroundings and manage chunk loading using an ActiveObject.

    public static Dictionary<string, Furniture> Loaded = new Dictionary<string, Furniture>();

    [Header("Basics")]
    public string Prefab;
    public string Name;

    [Header("Placement")]
    public string Layer = "Foreground";

    [Header("Details")]
    public ItemRarity Rarity = ItemRarity.COMMON;
    public Sprite Icon;

    [Header("Pathfinding")]
    public bool Walkable = false;

    public int X { get; private set; }
    public int Y { get; private set; }

    [HideInInspector]
    public ActiveObject AO;

    private TileLayer layer;

    public void Awake()
    {
        AO = GetComponent<ActiveObject>();
    }

    public override void OnStartClient()
    {
        // Have to detect X nad Y because they are not networked...
        X = Mathf.RoundToInt(transform.position.x);
        Y = Mathf.RoundToInt(transform.position.y);

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
            transform.position = new Vector3(X, Y);
            name = Name + " (" + X + ", " + Y + ")";
        }
    }

    public virtual FurnitureSaveData GetSaveData()
    {
        return new FurnitureSaveData(this);
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

    /// <summary>
    /// Gets a furniture PREFAB given the prefab name. Do not directly modify the return value.
    /// </summary>
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