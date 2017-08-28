using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GunEffects : MonoBehaviour
{
    public static GunEffects Instance;

    public Transform BulletTrail;

    public void Start()
    {
        Instance = this;
    }
}
