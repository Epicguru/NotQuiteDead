
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

public class BuildingMenuUI : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject Prefab;

    [Header("References")]
    public Transform ItemParent;
    public InputField IF;
    public Dropdown DD;

    private List<GameObject> spawned = new List<GameObject>();
    private bool isOpen;

    public void Start()
    {
        UpdateDropdownOptions();
    }

    public void Open()
    {
        if (isOpen)
            return;

        UI.FlagOpen();
        isOpen = true;

        Refresh();
    }

    public void Refresh()
    {
        // Test the functionality.
        BuildingItemData a = new BuildingItemData() { Name = "Bad Tile", Count = 10, Rarity = ItemRarity.RUBBISH };
        BuildingItemData b = new BuildingItemData() { Name = "OK Tile", Count = 5, Rarity = ItemRarity.COMMON };
        BuildingItemData c = new BuildingItemData() { Name = "Great Tile", Count = 12, Rarity = ItemRarity.VALUABLE };
        BuildingItemData d = new BuildingItemData() { Name = "Lol", Count = 1, Rarity = ItemRarity.GODLIKE };
        BuildingItemData e = new BuildingItemData() { Name = "Some Name", Count = 999, Rarity = ItemRarity.LEGENDARY };

        List<BuildingItemData> x = new List<BuildingItemData>();
        x.Add(a);
        x.Add(e);
        x.Add(c);
        x.Add(b);
        x.Add(d);

        Spawn(x, IF.text.Trim(), GetCurrentSorting());
    }

    public void Close()
    {
        if (!isOpen)
            return;

        UI.FlagClosed();

        isOpen = false;
        DestroySpawned();
    }

    public void DropdownChanged()
    {
        if(isOpen)
            Refresh();
    }

    public void InputFieldChanged()
    {
        if(isOpen)
            Refresh();
    }

    private void UpdateDropdownOptions()
    {
        DD.ClearOptions();
        List<string> op = new List<string>();
        foreach(BuildingMenuSorting s in Enum.GetValues(typeof(BuildingMenuSorting)))
        {
            op.Add(s.ToString());
        }
        DD.AddOptions(op);
    }

    public BuildingMenuSorting GetCurrentSorting()
    {
        try
        {
            var data = DD.options[DD.value];
            return (BuildingMenuSorting)Enum.Parse(typeof(BuildingMenuSorting), data.text);
        }
        catch
        {
            // Gotta catch em' all!
            return BuildingMenuSorting.NONE;
        }
    }

    public void Spawn(List<BuildingItemData> items, string filter, BuildingMenuSorting sorting)
    {
        DestroySpawned();

        if(items == null || items.Count == 0)
        {
            return;
        }

        Filter(items, filter);

        Spawn(items, sorting);
    }

    public void Spawn(List<BuildingItemData> items, BuildingMenuSorting sorting)
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
                s = from x in items orderby x.Count descending select x;
                items = s.ToList();
                break;
            case BuildingMenuSorting.COUNT_REVERSE:
                s = from x in items orderby x.Count ascending select x;
                items = s.ToList();
                break;
        }

        Spawn(items);
    }

    public void Filter(List<BuildingItemData> items, string filter)
    {
        if (items == null || items.Count == 0)
            return;

        string fLow = filter.ToLower();

        items.RemoveAll(x => !x.Name.ToLower().Contains(fLow));
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
            i.Count = item.Count;
            i.UpdateVisuals();
        }
    }

    public void DestroySpawned()
    {
        if (spawned.Count == 0)
            return;


        foreach(GameObject go in spawned)
        {
            Destroy(go);
        }

        spawned.Clear();
    }
}