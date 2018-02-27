using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(TileMap))]
public class World : NetworkBehaviour
{
    public static World Instance { get; private set; }

    public string RealityName;

    [HideInInspector]
    public TileMap TileMap;

    [HideInInspector]
    public FurnitureManager Furniture;

    public void Save()
    {
        // Save all tile layers to file.
        TileMap.SaveAll();

        // Save all world items to file.
        ItemIO.ItemsToFile(RealityName, GameObject.FindObjectsOfType<Item>());
    }

    public void Load()
    {
        // Load all world items to map.
        ItemIO.FileToWorldItems(RealityName, true);

        // No need to load tile layers, this is done all the time passively.
    }

    public void Awake()
    {
        RealityName = RealityName == null ? null : RealityName.Trim();
        if (string.IsNullOrEmpty(RealityName))
        {
            Debug.LogError("Reality name is null or empty! Invalid world for reality!");
            return;
        }

        Instance = this;

        TileMap = GetComponent<TileMap>();
        TileMap.World = this;

        Furniture = GetComponent<FurnitureManager>();

        TileMap.Create();
    }

    public void OnDestroy()
    {
        //Instance = null;
        Pawn.Dispose();
    }

    public void Update()
    {
        if (isServer)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                Load();
            }
        }
    }
}