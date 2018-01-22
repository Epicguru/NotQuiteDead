using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Spawnables : MonoBehaviour
{
    public static Spawnables I;

    public GameObject Arrow;
    public GameObject DamageNumber;

    public void Awake()
    {
        I = this;
    }

    public void OnDestroy()
    {
        if (I == this)
            I = null;
    }
}
