
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

public class BuildingMenuUI : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject Prefab;

    [Header("States")]
    public bool Dragging;

    [Header("References")]
    public Transform ItemParent;
    public InputField IF;
    public Dropdown DD;
    public Transform GhostParent;
    public Image BuildBarDropDropZone;

    [HideInInspector]
    public bool IsOpen;

    private List<GameObject> spawned = new List<GameObject>();
    private int actionID;

    public void Start()
    {
        UpdateDropdownOptions();
    }

    public void Update()
    {
        if (!IsOpen)
            return;

        if (Player.Local == null)
            return;

        if (actionID != Player.Local.BuildingInventory.ActionID)
        {
            Refresh();
        }

        // Update raycasting state for the drop zone based on the dragging status.
        BuildBarDropDropZone.raycastTarget = Dragging;
    }

    public void Open()
    {
        if (IsOpen)
            return;

        UI.FlagOpen();
        IsOpen = true;

        Refresh();
        Dragging = false;
    }

    public void Refresh()
    {
        // Rebuild the whole UI objects. Quite expensive so only done when necessary.
        if (Player.Local == null)
            return;

        List<BuildingItemData> x = new List<BuildingItemData>();

        var items = Player.Local.BuildingInventory.GetItems();

        foreach(var item in items)
        {
            x.Add(new BuildingItemData() { Name = item.Name, Prefab = item.Prefab, Rarity = item.GetRarity(), Count = item.Count, Icon = item.GetIcon() });
        }

        Spawn(x, IF.text.Trim(), GetCurrentSorting());

        actionID = Player.Local.BuildingInventory.ActionID;
    }

    public void Close()
    {
        if (!IsOpen)
            return;

        UI.FlagClosed();
        Dragging = false;

        foreach(Transform t in GhostParent)
        {
            Destroy(t.gameObject);
        }

        IsOpen = false;
        DestroySpawned();
    }

    public void DropdownChanged()
    {
        if(IsOpen)
            Refresh();
    }

    public void InputFieldChanged()
    {
        if(IsOpen)
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
            i.Prefab = item.Prefab;
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