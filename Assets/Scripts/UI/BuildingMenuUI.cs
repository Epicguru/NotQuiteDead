
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingMenuUI : MonoBehaviour
{
    public GameObject Prefab;
    public Transform ItemParent;

    private List<GameObject> spawned = new List<GameObject>();

    public void Open()
    {

    }

    public void Close()
    {

    }

    public void Spawn(List<BuildingItemData> items, BuildingMenuSorting sorting, bool reverse)
    {
        DestroySpawned();
        if (items == null || items.Count == 0)
        {
            return;
        }

        switch (sorting)
        {
            case BuildingMenuSorting.NONE:
                break;
            case BuildingMenuSorting.ALPHA:
                var s = from x in items orderby x.Name ascending select x;
                items = s.ToList();
                break;
            case BuildingMenuSorting.ALPHA_REVERSE:
                s = from x in items orderby x.Name descending select x;
                items = s.ToList();
                break;
            case BuildingMenuSorting.RARITY:
                s = from x in items orderby x.Rarity descending select x;
                items = s.ToList();
                break;
            case BuildingMenuSorting.RARITY_REVERSE:
                s = from x in items orderby x.Rarity ascending select x;
                items = s.ToList();
                break;
            case BuildingMenuSorting.COUNT:
                s = from x in items orderby x.Count ascending select x;
                items = s.ToList();
                break;
            case BuildingMenuSorting.COUNT_REVERSE:
                s = from x in items orderby x.Count descending select x;
                items = s.ToList();
                break;
        }

        Spawn(items);
    }

    public void Spawn(List<BuildingItemData> items)
    {
        DestroySpawned();

        if(items == null || items.Count == 0)
        {
            return;
        }

        foreach(var item in items)
        {
            GameObject go = Instantiate(Prefab, ItemParent);
            spawned.Add(go);
            BuildingMenuItem i = go.GetComponent<BuildingMenuItem>();
            i.Icon = item.Icon;
            i.BGColour = item.GetColour();
            i.Name = item.Name;
            i.UpdateVisuals();
        }
    }

    public void DestroySpawned()
    {
        foreach(GameObject go in spawned)
        {
            Destroy(go);
        }

        spawned.Clear();
    }
}