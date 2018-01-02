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

    public string Name;

    [HideInInspector]
    public TileMap TileMap;

    public void Awake()
    {
        TileMap = GetComponent<TileMap>();

        TileMap.Create();

        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }
}