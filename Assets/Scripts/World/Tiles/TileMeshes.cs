using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileMeshes : MonoBehaviour
{
    public Mesh Plane, Cube;

    public static TileMeshes Instance;

    public void Awake()
    {
        Debug.Log("Awake!");
        Instance = this;
    }
}
