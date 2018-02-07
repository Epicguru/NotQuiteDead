using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialProvider : MonoBehaviour
{
    public static MaterialProvider _Instance;

    public void Awake()
    {
        _Instance = this;
    }
}