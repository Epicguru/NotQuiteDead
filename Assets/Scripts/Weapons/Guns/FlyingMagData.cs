using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class FlyingMagData
{
    public Transform RealMag;
    public Sprite MagSprite;
    public float Force = 4f;
    public float RandomForce = 0f;
    public float Rotation = 20f;
    public float RandomRotation = 0f;
    public float Time = 1.5f;
    public int Count = 1;
}
