﻿
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBarUI : MonoBehaviour
{
    [Header("Code Only")]
    public int SelectedIndex;

    [Header("Editor Only")]
    public GameObject Prefab;
    public Transform ItemParent;
    public Text SelectedText;

    private List<BuildingBarItem> items = new List<BuildingBarItem>();
    private bool isOpen;

    public void Spawn(List<BuildingItem> items)
    {
        ClearSpawned();

        if(items == null || items.Count == 0)
        {
            return;
        }

        int index = 0;
        foreach(var x in items)
        {
            GameObject go = Instantiate(Prefab, ItemParent);
            BuildingBarItem item = go.GetComponent<BuildingBarItem>();

            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(5 + 55 * index, 0f);
            this.items.Add(item);

            index++;
        }
    }

    public void Open()
    {
        if (isOpen)
            return;

        isOpen = true;
        List<BuildingItem> i = new List<BuildingItem>(Player.Local.BuildingInventory.GetItems());
        Spawn(i);
    }

    public void Close()
    {
        if (!isOpen)
            return;

        isOpen = false;

        ClearSpawned();
    }

    public void Update()
    {
        if (!isOpen)
            return;

        HighlightIndex();
    }

    public void HighlightIndex()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].UpdateSelected(i == SelectedIndex);
            if(i == SelectedIndex)
            {
                if(Player.Local != null)
                    items[i].SetText(SelectedText, Player.Local.BuildingInventory.GetItem(items[i].Prefab).Count);
            }
        }
    }

    public void ClearSpawned()
    {
        if (items == null || items.Count == 0)
            return;


        foreach(var x in items)
        {
            Destroy(x.gameObject);
        }

        items.Clear();

        SelectedIndex = -1;
    }
}