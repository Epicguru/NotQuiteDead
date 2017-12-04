﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class World : NetworkBehaviour
{
    public string Name;

    public int Width, Height;

    [HideInInspector]
    public TileMap TileMap;

    public void Start()
    {
        TileMap = GetComponent<TileMap>();

        TileMap.Create(Width, Height);
    }
}