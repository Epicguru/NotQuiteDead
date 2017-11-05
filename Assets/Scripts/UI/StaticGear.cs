using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StaticGear : MonoBehaviour
{
    public GearUI Hands;
    public GearUI Head;
    public GearUI Chest;

    public void Awake()
    {
        Register(Hands);
        Register(Head);
        Register(Chest);
    }

    public static void Register(GearUI gear)
    {
        GearUI.GearItems.Add(gear.Title, gear);
    }
}
