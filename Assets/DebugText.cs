using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugText : MonoBehaviour {

    public static DebugText _Instance;

    public bool Active;

    public void Awake()
    {
        _Instance = this;
    }

    public void Update()
    {
        
    }
}