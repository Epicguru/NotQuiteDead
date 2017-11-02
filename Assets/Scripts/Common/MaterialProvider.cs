using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialProvider : MonoBehaviour
{
    public static MaterialProvider _Instance;

    public Material LitSprite;

    public void Awake()
    {
        _Instance = this;
    }
}