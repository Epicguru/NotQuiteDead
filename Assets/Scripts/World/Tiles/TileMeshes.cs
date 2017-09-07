using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileMeshes : MonoBehaviour
{
    public Mesh Quad, Cube;
    public Material LitSprite;

    public static TileMeshes Instance;

    public void Awake()
    {
        Instance = this;
    }
}
