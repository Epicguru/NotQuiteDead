using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class World : NetworkBehaviour
{
    public string Name;

    public MapLayer Map;

    public void Start()
    {
        Map.Create(100, 100);

        Map.LoadChunk(0, 0);
        Map.LoadChunk(1, 1);
    }
}