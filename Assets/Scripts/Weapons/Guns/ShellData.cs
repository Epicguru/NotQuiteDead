using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class ShellData
{
    public Transform ShellSpawn;

    public Sprite Sprite;

    public bool SpawnOnShoot = false;

    [Range(0f, 1f)]
    public float Random = 0.1f;

    public Vector2 Velocity = new Vector2(-3, 3);
    public Vector2 Gravity = new Vector2(0, -10);
    public float Rotation = 720f;
    public float Time = 2f;
}
