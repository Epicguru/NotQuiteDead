using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // An inventory with little to no physical structure, that has a limit of items.
    // The default limits are either:
    // 1. A maximum amount of items, or
    // 2. A maximum weight or possibly
    // 3. Both, a limit on items and weight.
    // EDIT - Lol this class is terrible.

    [Tooltip("The display name of the inventory.")]
    public string Name = "Default Name";

    [Tooltip("The inventory item prefab.")]
    public GameObject InventoryItemPrefab;

    [Tooltip("The details text object rendererd at the top right of the screen.")]
    public Text Details;

    [Tooltip("The title of the inventory.")]
    public Text Title;

    public QuickSlotInput QSI;

    [Tooltip("The details sub-screen.")]
    public InventoryDetailsView DetailsView;

    [Tooltip("The parent of the item game objects.")]
    public Transform ItemsParent;

    [Tooltip("The content object in the viewport of a scrollable.")]
    public RectTransform ViewportContent;

    [Tooltip("The maximum amount of items in this inventory. Negaive numbers means that there is no limit.")]
    public int MaxItems = -1; // If negative, ignored.

    [Tooltip("The maximum combined weight of all items in this inventory. Negaitve numbers means that there is no limit.")]
    public float MaxWeight = 100f; // If negative, ignored.

    public Dropdown SortingOption;
    public Dictionary<InventorySortingMode, string> SortingModeMap = new Dictionary<InventorySortingMode, string>();
    public Dictionary<string, InventorySortingMode> ReverseMap = new Dictionary<string, InventorySortingMode>();

    public ItemOptionsPanel Options;

    public List<InventoryItemData> Contents = new List<InventoryItemData>();

    private List<GameObject> Spawned = new List<GameObject>();

    public bool Refresh;

    public delegate void OnContentsChange();
    public event OnContentsChange ContentsChange;

    public void Awake()
    {
        SortingModeMap.Clear();
        SortingModeMap.Add(InventorySortingMode.NONE, "None");
        SortingModeMap.Add(InventorySortingMode.RARITY, "Rarity");
        SortingModeMap.Add(InventorySortingMode.COUNT, "Count");
        SortingModeMap.Add(InventorySortingMode.TYPE, "Item Type");
        SortingModeMap.Add(InventorySortingMode.QUICKSLOTTED, "Quickslot");
        SortingModeMap.Add(InventorySortingMode.ALPHA, "Alphabetical");

        ReverseMap.Clear();
        foreach(InventorySortingMode m in SortingModeMap.Keys)
        {
            ReverseMap.Add(SortingModeMap[m], m);
        }

        SetDowndownOptions();
    }

    public void Update()
    {
        if(Title.text != Name)
        {
            Title.text = Name;
        }

        if (Refresh)
        {
            // TODO sort.
            RefreshItems(Contents);
        }
    }

    public void Open()
    {
        RefreshItems(Contents);
    }

    public void DropdownChanged()
    {
        RefreshItems(Contents);
    }

    public void SetDowndownOptions()
    {
        SortingOption.ClearOptions();
        List<string> options = new List<string>();
        foreach(var x in SortingModeMap.Keys)
        {
            options.Add(SortingModeMap[x]);
        }
        SortingOption.AddOptions(options);
    }

    public void AddItem(string prefab, ItemData data, int amount = 1)
    {
        if (amount <= 0)
            return;
        if (string.IsNullOrEmpty(prefab))
            return;

        Item item = Item.GetItem(prefab);

        if(item == null)
        {
            return;
        }

        Refresh = true;

        if (CanAdd(item, amount))
        {
            // Do we already have a pile with those items?
            if (item.CanStack)
            {
                foreach(InventoryItemData d in Contents)
                {
                    if(d.Prefab == prefab)
                    {
                        // Found the pile!
                        d.Count += amount;

                        ContentsChange.Invoke();
                        return;
                    }
                }

                // We looked for a stack but didn't find one... I'll just make a new one.
                InventoryItemData ID = new InventoryItemData();
                ID.Prefab = prefab;
                ID.Data = data;
                ID.Count = amount;
                Contents.Insert(0, ID);

                ContentsChange.Invoke();
                return;
            }
            else
            {
                // Cool, don't need to check stacking.
                for (int i = 0; i < amount; i++)
                {
                    InventoryItemData ID = new InventoryItemData();
                    ID.Prefab = prefab;
                    ID.Data = data;
                    ID.Count = 1;
                    Contents.Insert(0, ID);
                }

                ContentsChange.Invoke();
                return;
            }
        }
        else
        {
            return;
        }
    }

    public void RefreshItems(List<InventoryItemData> items)
    {
        ClearSpawned();

        if (items == null || items.Count == 0)
            return;

        // Sort these items
        InventorySortingMode mode = GetSelectedSorting();
        List<InventoryItemData> sorted = Sort(items, mode);

        foreach(InventoryItemData d in sorted)
        {
            GameObject spawned = Instantiate(InventoryItemPrefab, ItemsParent);
            InventoryItem x = spawned.GetComponent<InventoryItem>();

            x.ItemData = d;
            x.SetText();

            Spawned.Add(spawned);
        }

        Refresh = false;
    }

    public InventorySortingMode GetSelectedSorting()
    {
        return ReverseMap[SortingOption.options[SortingOption.value].text];
    }

    public List<InventoryItemData> Sort(List<InventoryItemData> unsorted, InventorySortingMode mode)
    {
        List<InventoryItemData> alpha = (from x in unsorted orderby x.Item.Name select x).ToList();
        switch (mode)
        {
            case InventorySortingMode.NONE:
                return unsorted;
            case InventorySortingMode.COUNT:
                var s = from x in alpha orderby x.Count descending select x;
                return s.ToList();
            case InventorySortingMode.RARITY:
                s = from x in alpha orderby x.Item.Rarity descending select x;
                return s.ToList();
            case InventorySortingMode.QUICKSLOTTED:
                s = from x in alpha orderby (x.Data == null ? -1 : (x.Data.Get<int>("Quick Slot") == 0 ? -1 : (100 - x.Data.Get<int>("Quick Slot")))) descending select x;
                return s.ToList();
            case InventorySortingMode.TYPE:
                return unsorted;
            case InventorySortingMode.ALPHA:
                return alpha;
            default:
                return unsorted;
        }
    }

    public void ClearSpawned()
    {
        if (Spawned.Count == 0)
            return;

        foreach(var g in Spawned)
        {
            Destroy(g);
        }
        Spawned.Clear();
    }

    public int RemoveItem(string prefab, Vector2 position, bool drop = true, int amount = 1)
    {
        // Find the item for that prefab.
        Item item = Item.GetItem(prefab);
        if(item == null)        
            return 0;        
        if (amount <= 0)
            return 0;

        Refresh = true;

        if (item.CanStack)
        {
            // Item can stack, find item in inventory and change the item count.
            for(int y = 0; y < Contents.Count; y++)
            {
                var i = Contents[y];
                if(i.Prefab == prefab)
                {
                    int removed = 0;
                    // Found it!
                    for (int x = 0; x < amount; x++)
                    {
                        if (drop)
                        {
                            // Create new instance...
                            // No need to apply data, because it is up to date.
                            Vector2 offset = Random.insideUnitCircle * 0.1f;
                            Player.Local.NetUtils.CmdSpawnDroppedItem(i.Prefab, position + offset, null);
                        }
                        i.Count -= 1;
                        removed += 1;
                        if(i.Count <= 0)
                        {
                            // Cannot remove any more, stop!
                            Contents.RemoveAt(y);

                            ContentsChange.Invoke();
                            return removed;
                        }
                    }
                }
            }

            ContentsChange.Invoke();
            return 0;
        }
        else
        {
            // Items does not stack so if there are any in the inventory then they will be split up.
            int removed = 0;

            List<InventoryItemData> bin = new List<InventoryItemData>();
            foreach(var d in Contents)
            {
                if(d.Prefab == prefab)
                {
                    // Remove it! We know that it is only one item in the stack because it does not stack!
                    removed++;
                    bin.Add(d);

                    if (drop)
                    {
                        if (d.Data != null)
                            d.Data.Update("Quick Slot", 0);
                        Vector2 offset = Random.insideUnitCircle * 0.1f;
                        Player.Local.NetUtils.CmdSpawnDroppedItem(d.Prefab, position + offset, d.Data == null ? null : d.Data.Serialize());
                    }

                    if(removed == amount)
                    {
                        break;
                    }
                }
            }
            foreach(var thing in bin)
            {
                Contents.Remove(thing);
            }

            ContentsChange.Invoke();
            return removed;
        }
    }

    public InventoryItemData GetOfType(string prefab)
    {
        foreach (InventoryItemData item in Contents)
        {
            if (item.Prefab == prefab)
                return item;
        }
        return null;
    }

    public bool Contains(string prefab, int amount)
    {
        int count = 0;
        foreach (InventoryItemData item in Contents)
        {
            if(item.Prefab == prefab)
            {
                count += item.Count;
            }
            if (count >= amount)
                return true;
        }
        if (count >= amount)
            return true;
        return false;
    }

    public void Clear()
    {
        // Removes all items stacks. Poof. They vanish.
        while(Contents.Count > 0)
        {
            RemoveItem(Contents[0].Prefab, Vector2.zero, false, Contents[0].Count);
        }
    }

    public void DropAll(float radius, Vector2 position)
    {
        // Drop all items and scatter them around the position.
        while (Contents.Count > 0)
        {
            Vector2 offset = Random.insideUnitCircle * 2f;

            RemoveItem(Contents[0].Prefab, position + offset, true, Contents[0].Count);
        }
    }

    public bool CanAdd(Item i, int amount)
    {
        return true;
    }

    public int GetCombinedItems()
    {
        int x = 0;
        foreach(InventoryItemData i in Contents)
        {
            x += i.Count;
        }

        return x;
    }

    public float GetCombinedWeight()
    {
        float weight = 0;

        foreach(InventoryItemData i in Contents)
        {
            weight += i.Item.InventoryInfo.Weight * i.Count;
        }

        return weight;
    }
}

public enum InventorySortingMode
{
    NONE,
    RARITY,
    COUNT,
    QUICKSLOTTED,
    TYPE,
    ALPHA
}