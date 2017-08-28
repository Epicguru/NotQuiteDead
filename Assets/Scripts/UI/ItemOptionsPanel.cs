using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemOptionsPanel : MonoBehaviour
{
    public GameObject Prefab;
    public Transform Parent;

    private const float START_Y = -40f;
    private const float INCREMENT = -30f;
    private List<GameObject> spawns = new List<GameObject>();

    public void Open(InventoryItem item)
    {
        List<ItemOption> options = item.Item.Options;

        int index = 0;
        foreach(ItemOption option in options)
        {
            float y = START_Y + INCREMENT * index;

            GameObject GO = Instantiate(Prefab);

            index++;
        }
    }
}
