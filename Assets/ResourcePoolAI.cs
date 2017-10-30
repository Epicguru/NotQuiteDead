using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePoolAI : MonoBehaviour
{
    public GameObject Prefab;
    private List<ResourceAmount> Resources = new List<ResourceAmount>();

    public void Refresh()
    {

    }
}

public struct ResourceAmount
{
    public Item Item;
    public int Amount;
}