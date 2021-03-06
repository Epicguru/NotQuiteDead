﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(TileMap))]
public class World : NetworkBehaviour
{
    public static World Instance { get; private set; }

    [Header("References")]
    public GameTime GameTime;

    [Header("World Specific")]
    public string RealityName;
    public Vector2 SpawnPoint;
    public float SpawnRadius = 10f;

    [HideInInspector]
    public TileMap TileMap;

    [HideInInspector]
    public FurnitureManager Furniture;

    [Server]
    public void Save()
    {
        if (!isServer)
            return;

        // Save all tile layers to file.
        TileMap.SaveAll();

        // Save the player inventory:
        // This will only save the host's inventory. TODO support other clients saving inventory data.
        InventoryIO.SaveInventory(RealityName);

        // Save current gear held and worn by local player. TODO see above.
        InventoryIO.SaveGear(RealityName, Player.Local);

        // Save currently held item. TODO see above.
        InventoryIO.SaveHolding(RealityName, Player.Local);

        // Save player state (position, health, etc.)
        PlayerIO.SavePlayerState(RealityName, Player.Local);

        // Save all world items to file.
        ItemIO.ItemsToFile(RealityName, GameObject.FindObjectsOfType<Item>());

        // Save placed furniture...
        FurnitureIO.SaveFurniture(RealityName, Furniture.GetAllFurniture());

        // Save the building inventory...
        BuildingIO.SaveBuildingInventory(RealityName, Player.Local);

        // Save the world state to file.
        WorldIO.SaveWorldState(this);
    }

    [Server]
    public void Load()
    {
        if (!isServer)
            return;

        // Load world state from file.
        WorldIO.LoadWorldState(this);

        // Load player state (position, health, etc.)
        PlayerIO.LoadPlayerState(RealityName, Player.Local);

        // Load all world items to map.
        ItemIO.FileToWorldItems(RealityName, true);

        // Load the player inventory.
        // This will only load the host's inventory. TODO support other clients loading inventory data.
        InventoryIO.LoadInventory(RealityName);

        // Save current gear held and worn by local player. TODO see above.
        InventoryIO.LoadGear(RealityName, Player.Local);

        // Load currently held item. TODO see above.
        InventoryIO.LoadHolding(RealityName, Player.Local);

        // Load furniture...
        FurnitureIO.LoadFurniture(this);

        // Load player building inventory...
        BuildingIO.LoadBuildingInventory(RealityName, Player.Local);

        // No need to load tile layers, this is done all the time passively.

        // Call post-load
        PostLoad();
    }

    public void PostLoad()
    {
        if (SpawnPoint == Vector2.zero)
        {
            // Tilemap has a width and height: Use it to determine the spawn point 
            // Place the spawn point right in the middle of the world.
            float width = TileMap.Width;
            float height = TileMap.Height;

            // Get the center...
            float x = width / 2f;
            float y = height / 2f;

            SpawnPoint = new Vector2(x, y);
        }
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

    public void Start()
    {
        // Generate when new?
        PostLoad();
    }

    public Vector2 GetRandomSpawnPoint()
    {
        Vector2 offset = SpawnRadius == 0f ? Vector2.zero : UnityEngine.Random.insideUnitCircle * SpawnRadius;
        Vector2 point = SpawnPoint;

        return point + offset;
    }

    public void OnDestroy()
    {
        //Instance = null;
        Pawn.Dispose();
    }
}