using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StaticGear : MonoBehaviour
{
    public GearItem Hands;

    public void Awake()
    {
        GearItem.GearItems.Add(Hands.Title, Hands);
    }
}
