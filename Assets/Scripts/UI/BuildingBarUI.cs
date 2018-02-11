
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
    public RectTransform Panel;

    public List<BuildingItem> Items = new List<BuildingItem>();

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
            item.Prefab = x.Prefab;
            item.Icon = x.GetIcon();
            item.Name = x.Name;
            item.UpdateVisuals(false);

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
        Refresh();
    }

    public void Refresh()
    {
        Spawn(Items);
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

        UpdateSelected();
        UpdatePanelSize();

        if (Items.Count == 0)
            SelectedText.text = "---";
    }

    public void UpdatePanelSize()
    {
        // Size.x = 10 + 55 * items (min 250)
        // Size.y = 90

        float x = Mathf.Clamp(10f + 55f * Items.Count, 250, float.MaxValue);
        float y = 90;
        Vector2 size = new Vector2(x, y);

        Panel.sizeDelta = size;
    }

    public void UpdateSelected()
    {
        SelectedIndex = Mathf.Clamp(SelectedIndex, 0, items.Count - 1);

        if (items.Count == 0)
            return;

        bool inInventory = Player.Local.BuildingInventory.ContainsItem(items[SelectedIndex].Prefab);

        if (!inInventory)
        {
            Refresh();
            return;
        }

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