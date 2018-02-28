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

    public string RealityName;

    [HideInInspector]
    public TileMap TileMap;

    [HideInInspector]
    public FurnitureManager Furniture;

    [Server]
    public void Save()
    {
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
    }

    [Server]
    public void Load()
    {
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