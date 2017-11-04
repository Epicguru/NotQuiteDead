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

    [HideInInspector]
    public List<InventoryItem> Contents = new List<InventoryItem>();
    private float weight;
    private int items;
    [HideInInspector] public ItemOptionsPanel Options;

    public delegate void OnContentsChange();
    public event OnContentsChange ContentsChange;

    public void Start()
    {
        Options = GetComponentInChildren<ItemOptionsPanel>(true);
    }

    public void Update()
    {
        // TODO make less frequent.
        Details.text = "Items: " + items + "/" + (MaxItems < 0 ? "---" : MaxItems.ToString()) + "\nWeight: " + weight + "/" +  (MaxWeight < 0 ? "---" : MaxWeight.ToString() + "Kg");

        if(Title.text != Name)
        {
            Title.text = Name;
        }
    }

    public void ResetViewport()
    {
        ViewportContent.anchoredPosition = Vector2.zero;

        foreach(InventoryItem i in Contents)
        {
            i.SetText();
        }
    }

    public InventoryItem AddItem(string prefab, ItemData data, int amount = 1)
    {
        Item item = Item.FindItem(prefab);
        if (CanAdd(item, amount))
        {
            // Add this item to the inventory.
            // Make the InventoryItem prefab and set it up.
            // Also TODO configure the size of the inventory in UI.

            bool needNewRow = !item.CanStack;

            if (needNewRow || GetOfType(item.Prefab) == null)
            {
                GameObject spawned = Instantiate(InventoryItemPrefab, ItemsParent);
                RectTransform rect = spawned.GetComponent<RectTransform>();
                InventoryItem i = spawned.GetComponent<InventoryItem>();

                // For now, all items are separated and cannot be stacked: TODO.

                // Configure item.
                i.ItemPrefab = item.Prefab;
                i.Inventory = this;
                i.ItemCount = amount;

                // Place at bottom
                rect.anchoredPosition = new Vector2(0, -Contents.Count * 30);

                i.Init(data); // Data may be null, who knows!

                // Add item
                Contents.Add(i);

                // Add weight
                weight += i.Item.InventoryInfo.Weight;

                // Extend viewport
                ViewportContent.sizeDelta = new Vector2(0, Contents.Count * 30);

                // Add item count
                items += amount;

                if (ContentsChange != null)
                    ContentsChange();

                return i;
            }
            else
            {
                // No item data because items can stack.
                InventoryItem stack = GetOfType(item.Prefab);
                stack.SetItemCount(stack.ItemCount + amount);

                // Add weight
                weight += stack.Item.InventoryInfo.Weight * amount;

                // Add item count
                items += amount;

                if (ContentsChange != null)
                    ContentsChange();

                return stack;
            }
        }
        else
        {
            return null;
        }
    }

    public void RemoveItem(InventoryItem item, Vector2 position, bool drop = true, int amount = 1)
    {
        int index = this.Contents.IndexOf(item);

        bool lastItem = Contents[index].ItemCount <= amount;

        // Item count
        items -= amount;

        if(Contents[index].ItemCount < amount)
        {
            Debug.LogError("Cannot remove that many items : there are only " + Contents[index].ItemCount);
            // Do not return, just drop as many as we have!
            amount = Contents[index].ItemCount;
        }

        weight -= item.Item.InventoryInfo.Weight * amount;

        if(lastItem)
            this.Contents.Remove(item);

        // Spawn item on ground
        if (drop)
        {
            // Create new instance...
            // No need to apply data, because it is up to date.
            for (int i = 0; i < amount; i++)
            {
                if(item.Data != null)
                    item.Data.QuickSlot = 0;
                Player.Local.NetUtils.CmdSpawnDroppedItem(item.Item.Prefab, position + new Vector2(Random.Range(-0.05f * i, 0.05f * i), Random.Range(-0.05f * i, 0.05f * i)), item.Data == null ? new ItemData() : item.Data);
            }
        }

        if (!lastItem)
        {
            Contents[index].SetItemCount(Contents[index].ItemCount - amount);
        }

        if (ContentsChange != null)
            ContentsChange();

        if (!lastItem)
            return;

        Destroy(item.gameObject);

        // Move everything below this upwards, and make viewport smaller.
        for(int i = index; i < Contents.Count; i++)
        {
            RectTransform x = Contents[i].GetComponent<RectTransform>();
            x.anchoredPosition = new Vector2(x.localPosition.x + 100, x.localPosition.y + 30);
        }

        // Viewport
        ViewportContent.sizeDelta = new Vector2(0, ViewportContent.sizeDelta.y - 30);
    }

    public InventoryItem GetOfType(string prefab)
    {
        foreach (InventoryItem item in Contents)
        {
            if (item.ItemPrefab == prefab)
                return item;
        }
        return null;
    }

    public bool Contains(Item type, int amount)
    {
        int count = 0;
        foreach (InventoryItem item in Contents)
        {
            if(item.ItemPrefab == type.Prefab)
            {
                count += item.ItemCount;
            }
            if (count >= amount)
                return true;
        }
        return false;
    }

    public void Clear()
    {
        // Removes all items stacks. Poof. They vanish.
        while(Contents.Count > 0)
        {
            RemoveItem(Contents[0], Vector2.zero, false, Contents[0].ItemCount);
        }
    }

    private Vector2 offset = new Vector2();
    public void DropAll(float radius, Vector2 position)
    {
        // Drop all items and scatter them around the position.
        while (Contents.Count > 0)
        {
            float angle = UnityEngine.Random.Range(0f, 360f);
            float r = UnityEngine.Random.Range(0, radius);

            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * r;
            float y = Mathf.Sin(angle * Mathf.Deg2Rad) * r;

            offset.Set(x, y);

            RemoveItem(Contents[0], position + offset, true, Contents[0].ItemCount);

        }
    }

    public bool CanAdd(Item i, int amount)
    {
        return CanAddWeight(i, amount) && CanAddCapacity(i, amount);
    }

    public bool CanAddCapacity(Item i, int amount)
    {
        return this.CanAddCapacity(amount);
    }

    public bool CanAddCapacity(int extra)
    {
        if (MaxItems < 0)
            return true;

        return Contents.Count + extra <= MaxItems;           
    }

    public bool CanAddWeight(Item item, int amount)
    {
        return this.CanAddWeight(item.InventoryInfo.Weight * amount);
    }

    public bool CanAddWeight(float weight)
    {
        if (MaxWeight < 0)
            return true;
        return this.weight + weight <= MaxWeight;
    }

    public int GetCombinedItems()
    {
        int x = 0;
        foreach(InventoryItem i in Contents)
        {
            x += i.ItemCount;
        }

        return x;
    }

    public float GetCombinedWeight()
    {
        float weight = 0;

        foreach(InventoryItem i in Contents)
        {
            weight += i.Item.InventoryInfo.Weight * i.ItemCount;
        }

        return weight;
    }
}