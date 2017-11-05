using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Item))]
public class Furniture : NetworkBehaviour
{
    public RectInt Size;
    public bool Solid = true;
}