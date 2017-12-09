using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(TileMap))]
public class World : NetworkBehaviour
{
    public string Name;

    [HideInInspector]
    public TileMap TileMap;

    public void Start()
    {
        TileMap = GetComponent<TileMap>();

        TileMap.Create();
    }
}