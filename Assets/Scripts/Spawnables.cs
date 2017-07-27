using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnables : MonoBehaviour {

    public static Spawnables I;

    public VisualBullet BulletTrail;

    public void Start()
    {
        I = this;
    }
}